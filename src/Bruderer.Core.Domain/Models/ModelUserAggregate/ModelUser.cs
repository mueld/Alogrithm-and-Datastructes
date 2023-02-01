using Bruderer.Core.Domain.Messaging.Response;
using Bruderer.Core.Domain.Models.ComponentAggregate;
using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Arguments;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Response;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelUserAggregate
{
    public abstract class ModelUser : IModelUser
    {
        #region fields

        protected readonly ILoggerFactory _LoggerFactory;
        private readonly ILogger _Logger;

        protected ModelUserStateEnumeration _LastState = ModelUserStateEnumeration.Unknown;

        #endregion
        #region ctor

        public ModelUser(
            string name,
            ILoggerFactory loggerFactory)
        {
            Name = name;

            _LoggerFactory = loggerFactory;
            _Logger = loggerFactory.CreateLogger<ModelUser>();
        }

        #endregion
        #region methods

        /// <summary>
        /// Initializes this model user.
        /// </summary>
        /// <remarks>
        /// This method provides a way to integrate model user specific start conditions and will be automatically executed by the <see cref="StartAsync"/> method.
        /// </remarks>
        /// <param name="cancellationToken">CancellationToken for the task.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when initializing of this model user failed.
        /// </exception>
        protected abstract Task Initialize(CancellationToken cancellationToken);

        /// <summary>
        /// Terminates this model user.
        /// </summary>
        /// <remarks>
        /// This method provides a way to integrate model user specific stop conditions and will be automatically executed by the <see cref="StopAsync"/> method.
        /// </remarks>
        /// <param name="cancellationToken">CancellationToken for the task.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when terminating of this model user failed.
        /// </exception>
        protected abstract Task Terminate(CancellationToken cancellationToken);

        protected virtual Task SetState(ModelUserStateEnumeration newState, CancellationToken cancellationToken)
        {
            // Set state
            _LastState = State;
            State = newState;
            _Logger.LogInformation($"Model user [{Name}] changed state to [{newState}].");

            return Task.CompletedTask;
        }

        protected async Task UpdateComponentData(
            ComponentStateEnumeration state,
            CancellationToken cancellationToken)
        {
            await UpdateComponentData(Component.Name, Component.Version, state, Component.MessageStack, cancellationToken);
        }

        protected async Task UpdateComponentData(
            List<IResponseMessage> messageStack,
            CancellationToken cancellationToken)
        {
            await UpdateComponentData(Component.Name, Component.Version, Component.State, messageStack, cancellationToken);
        }

        protected virtual Task UpdateComponentData(
            string name,
            string version,
            ComponentStateEnumeration state,
            List<IResponseMessage> messageStack,
            CancellationToken cancellationToken)
        {
            // Set new component data
            Component.Name = name;
            Component.Version = version;
            Component.State = state;
            Component.MessageStack = messageStack;

            _Logger.LogInformation($"Model user [{Name}] has updated component data [{name}, {state}].");

            return Task.CompletedTask;
        }

        #endregion

        #region IModelUser

        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; private set; } = string.Empty;
        public ModelUserPlatformEnumeration Platform { get; protected set; } = ModelUserPlatformEnumeration.Unknown;
        public ModelUserStateEnumeration State { get; private set; } = ModelUserStateEnumeration.Unknown;
        public IComponent Component { get; private set; } = new Component();

        public abstract Task StartAsync(CancellationToken cancellationToken);
        public abstract Task StopAsync(CancellationToken cancellationToken);

        public abstract Task<bool> ConsumeModelData(ModelUpdateOptions modelUpdateOptions, CancellationToken cancellationToken);
        public abstract Task<bool> ReleaseModelData(ModelUpdateOptions modelUpdateOptions, CancellationToken cancellationToken);

        public abstract Task<bool> PushVariableData(IEnumerable<IModelVariableDEO> modelVariableDEOs, CancellationToken cancellationToken);
        public abstract Task<Dictionary<Guid, List<IModelVariableDEO>>> PullVariableData(CancellationToken cancellationToken);
        public abstract Task<Dictionary<Guid, List<IModelVariableDEO>>> PullVariableData(List<Guid> variableIds, CancellationToken cancellationToken);

        public abstract Task<ModelRPCResponse> InvokeRpcMethod(Guid rpcId, IList<object> inputArguments, CancellationToken cancellationToken);
        public abstract Task<RPCResponse<T>> InvokeRpcMethod<T>(Guid rpcId, IList<object> inputArguments, CancellationToken cancellationToken)
            where T : ModelRPCOutputArgumentContainer;

        #endregion
    }
}
