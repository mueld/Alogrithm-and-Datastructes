using Bruderer.Core.Domain.Commands;
using Bruderer.Core.Domain.Events;
using Bruderer.Core.Domain.Interfaces;
using Bruderer.Core.Domain.Messaging.Response;
using Bruderer.Core.Domain.Models.ComponentAggregate;
using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Infrastructure.PLC.Connection;
using Bruderer.Core.Infrastructure.PLC.Events;
using Bruderer.Core.Infrastructure.Services.BackgroundServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bruderer.Core.Infrastructure.PLC
{
    public abstract class PLCModelUser : ModelUser, IPLCModelUser
    {
        #region fields

        protected readonly IServiceScopeFactory _ServiceScopeFactory;
        protected readonly IModelFactory _ModelFactory;
        private readonly ILogger<PLCModelUser> _Logger;
        protected readonly IConfiguration _Configuration;
        protected BackgroundLoopService _ProcessingLoopService;

        #endregion
        #region ctor

        public PLCModelUser(
            string name,
            IServiceScopeFactory serviceScopeFactory,
            IModelFactory modelFactory,
            ILoggerFactory loggerFactory,
            IConfiguration configuration)
            : base(name, loggerFactory)
        {
            _ServiceScopeFactory = serviceScopeFactory;
            _ModelFactory = modelFactory;
            _Logger = _LoggerFactory.CreateLogger<PLCModelUser>();
            _Configuration = configuration;

            // Setup the processing loop service
            // Loop time should be at least a subset of the available model sampling rates. Default is 100ms.
            _ProcessingLoopService = new BackgroundLoopService(ProcessLoop);
        }

        #endregion
        #region props
        #endregion
        #region methods

        /// <summary>
        /// Provides a way to execute cyclic PLC processes and will be automatically executed based on the information on the <see cref="ProcessingPeriod"/> property.
        /// </summary>
        /// <remarks>
        /// This method is provided by a <see cref="BackgroundLoopService"/> class.
        /// </remarks>
        /// <param name="cancellationToken">CancellationToken for the task.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when processing the loop failed.
        /// </exception>
        protected abstract Task ProcessLoop(CancellationToken cancellationToken, long ticksByStart);

        /// <summary>
        /// Sets the connection state of this PLC model user. The state is represented in the <see cref="ConnectionState"/> property.
        /// </summary>
        /// <remarks>
        /// Publishes a <see cref="PLCConnectionStateChangedEvent"/> event which can be catched via the regular flow in a corresponding event handler. Invokes also a change of the component state with the <see cref="UpdateComponentData"/> method.
        /// </remarks>
        /// <param name="newState">New connection state of the PLC.</param>
        /// <param name="cancellationToken">CancellationToken for the task.</param>
        protected async Task SetConnectionState(PLCConnectionStateEnumeration newState, CancellationToken cancellationToken)
        {
            // Set state
            var oldState = ConnectionState;
            ConnectionState = newState;
            _Logger.LogInformation($"PLC model user [{Name}] changed connection state to [{newState}].");

            // First we map the PLC connection state to a model user state and perform a state update if needed
            var newModelUserState = (newState == PLCConnectionStateEnumeration.Connected) ? ModelUserStateEnumeration.Ok : ModelUserStateEnumeration.Error;
            if (newModelUserState != State)
                await SetState(newModelUserState, cancellationToken);

            // Publish plc connection state changed event
            using var scope = _ServiceScopeFactory.CreateScope();
            var messageBus = scope.ServiceProvider.GetService<IMessageBus>();

            // Setup and dispatch event
            var plcConnectionStateChangedEvent = new PLCConnectionStateChangedEvent(Id, newState, oldState);
            plcConnectionStateChangedEvent.ModelUserName = Name;

            await messageBus.PublishEvent(plcConnectionStateChangedEvent, cancellationToken);
        }

        /// <summary>
        /// Sets the runtime state of this PLC model user. The state is represented in the <see cref="RuntimeState"/> property.
        /// </summary>
        /// <remarks>
        /// Publishes a <see cref="PLCRuntimeStateChangedEvent"/> event which can be catched via the regular flow in a corresponding event handler. Invokes also a change of the component state with the <see cref="UpdateComponentData"/> method.
        /// </remarks>
        /// <param name="newState">New runtime state of the PLC.</param>
        /// <param name="cancellationToken">CancellationToken for the task.</param>
        protected async Task SetRuntimeState(PLCRuntimeStateEnumeration newState, CancellationToken cancellationToken)
        {
            // Set state
            var oldState = RuntimeState;
            RuntimeState = newState;
            _Logger.LogInformation($"PLC model user [{Name}] changed runtime state to [{newState}].");

            // First we map the PLC running state to a model user state and perform a state update if needed
            var newModelUserState = (newState == PLCRuntimeStateEnumeration.Running) ? ModelUserStateEnumeration.Ok : ModelUserStateEnumeration.Error;
            if (newModelUserState != State)
                await SetState(newModelUserState, cancellationToken);

            // Publish plc runtime state changed event
            using var scope = _ServiceScopeFactory.CreateScope();
            var messageBus = scope.ServiceProvider.GetService<IMessageBus>();

            // Setup and dispatch event
            var plcConnectionStateChangedEvent = new PLCRuntimeStateChangedEvent(Id, newState, oldState);
            plcConnectionStateChangedEvent.ModelUserName = Name;

            await messageBus.PublishEvent(plcConnectionStateChangedEvent, cancellationToken);
        }

        #endregion

        #region ModelUser

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            // Check if all used models are 'Running'
            var usedModels = _ModelFactory.GetUsedModels(Id);
            var areModelsStarted = usedModels.All(model => model.State == ModelStateEnumeration.Ok);

            // Invoke state update
            await SetState(ModelUserStateEnumeration.Started, cancellationToken);

            // Initialize derived functions
            await Initialize(cancellationToken);

            // Invoke state update
            await SetState(ModelUserStateEnumeration.Ok, cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // Check if all used models are 'Stopped'
            var usedModels = _ModelFactory.GetUsedModels(Id);
            var areModelsStopped = usedModels.All(model => model.State == ModelStateEnumeration.Stopped);

            // Invoke state update
            await SetState(ModelUserStateEnumeration.Stopped, cancellationToken);

            // Terminate derived functions
            await Terminate(cancellationToken);
        }

        protected override async Task SetState(ModelUserStateEnumeration newState, CancellationToken cancellationToken)
        {
            await base.SetState(newState, cancellationToken);

            // Publish model state changed event
            using var scope = _ServiceScopeFactory.CreateScope();
            var messageBus = scope.ServiceProvider.GetService<IMessageBus>();

            // Setup and dispatch event
            var modelStateChangedEvent = new ModelUserStateChangedEvent(Id, State, _LastState);
            await messageBus.PublishEvent(modelStateChangedEvent, cancellationToken);

            // Here we map the model user state to a component state and perform a state update if needed
            var newComponentState = (newState == ModelUserStateEnumeration.Ok) ? ComponentStateEnumeration.Ok : ComponentStateEnumeration.Error;
            if (newComponentState != Component.State)
                await UpdateComponentData(newComponentState, cancellationToken);
        }

        protected override async Task UpdateComponentData(
            string name,
            string version,
            ComponentStateEnumeration state,
            List<IResponseMessage> messageStack,
            CancellationToken cancellationToken)
        {
            await base.UpdateComponentData(name, version, state, messageStack, cancellationToken);

            // Send update component command
            using var scope = _ServiceScopeFactory.CreateScope();
            var messageBus = scope.ServiceProvider.GetService<IMessageBus>();

            // Setup command
            var command = new ComponentChangeCommand(Component);
            command.CallerContext.Id = Id;
            command.CallerContext.Name = Name;

            // Dispatch command
            await messageBus.SendCommand(command, cancellationToken);
        }

        #endregion

        #region IPLCModelUser

        public PLCConnectionStateEnumeration ConnectionState { get; private set; }
        public PLCRuntimeStateEnumeration RuntimeState { get; private set; }
        public TimeSpan ProcessingPeriod
        {
            get
            {
                return _ProcessingLoopService.Period;
            }
            set
            {
                _ProcessingLoopService.Period = value;

            }
        }
        public List<IModelVariable> UnlinkedVariables { get; protected set; } = new List<IModelVariable>();
        public int ConnectionCounter { get; protected set; } = 0;
        public HashSet<Guid> AddedModelVariableIds { get; protected set; } = new HashSet<Guid>();

        #endregion
    }
}
