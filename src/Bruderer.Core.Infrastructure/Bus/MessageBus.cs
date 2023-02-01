using Bruderer.Core.Domain.Interfaces;
using Bruderer.Core.Domain.Messaging;
using Bruderer.Core.Domain.Messaging.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Bruderer.Core.Infrastructure.Bus
{
    public class MessageBus : IMessageBus
    {
        #region fields

        private readonly ILogger<MessageBus> _Logger;
        private readonly IMediator _Mediator;
        private readonly IEventStore _EventStore;

        #endregion
        #region ctor

        public MessageBus(
            ILogger<MessageBus> logger,
            IMediator mediator,
            IEventStore eventStore)
        {
            _Logger = logger;
            _Mediator = mediator;
            _EventStore = eventStore;
        }

        #endregion
        #region methods

        private void SetupLog(Message message)
        {
            var messageType = message.MessageType;

            var callerInfo = message.CallerContext.Name;
            if (string.IsNullOrEmpty(callerInfo))
                callerInfo = "UnknownCaller";

            if (message is ICommand)
            {
                if (message is IModelCommand)
                {
                    var modelCommand = message as IModelCommand;
                    var modelInfo = modelCommand.ModelName;
                    if (string.IsNullOrEmpty(modelInfo))
                        modelInfo = modelCommand.ModelId.ToString();

                    if (message is IModelRpcCommand)
                    {
                        _Logger.LogDebug($"[{callerInfo}] is sending rpc command [{messageType}] for model [{modelInfo}]. {message.MessageInfo}");
                        return;
                    }

                    _Logger.LogDebug($"[{callerInfo}] is sending command [{messageType}] for model [{modelInfo}]. {message.MessageInfo}");
                    return;
                }

                _Logger.LogDebug($"[{callerInfo}] is sending command [{messageType}]. {message.MessageInfo}");
                return;
            }
            else if (message is IEvent)
            {
                if (message is IModelEvent)
                {
                    var modelEvent = message as IModelEvent;
                    var modelInfo = modelEvent.ModelName;
                    if (string.IsNullOrEmpty(modelInfo))
                        modelInfo = modelEvent.ModelId.ToString();

                    _Logger.LogDebug($"[{callerInfo}] is publishing event [{messageType}] for model [{modelInfo}]. {message.MessageInfo}");
                    return;
                }
            }
        }

        #endregion

        #region IMessageBus

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual Task<IResponse<bool>> SendCommand<T>(T command, CancellationToken cancellationToken = default)
            where T : Command
        {
            SetupLog(command);
            return _Mediator.Send(command, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual Task<IResponse<TResponse>> SendCommand<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : Command<TResponse>
        {
            SetupLog(command);
            return _Mediator.Send(command, cancellationToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual Task PublishEvent<T>(T @event, CancellationToken cancellationToken = default)
            where T : Event
        {
            _EventStore?.Save(@event);

            SetupLog(@event);
            return _Mediator.Publish(@event, cancellationToken);
        }

        #endregion
    }
}
