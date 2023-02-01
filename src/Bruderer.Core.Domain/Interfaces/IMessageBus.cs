using Bruderer.Core.Domain.Messaging;
using Bruderer.Core.Domain.Messaging.Response;
using System.Threading;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Interfaces
{
    public interface IMessageBus
    {
        Task<IResponse<bool>> SendCommand<T>(T command, CancellationToken cancellationToken = default) where T : Command;

        Task<IResponse<TResponse>> SendCommand<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : Command<TResponse>;

        Task PublishEvent<T>(T @event, CancellationToken cancellationToken = default) where T : Event;
    }
}
