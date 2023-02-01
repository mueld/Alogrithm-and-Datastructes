using Bruderer.Core.Application.Interfaces;
using Bruderer.Core.Domain.Commands;
using Bruderer.Core.Domain.Events;
using Bruderer.Core.Domain.Interfaces;
using Bruderer.Core.Domain.Messaging;
using Bruderer.Core.Domain.Messaging.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Bruderer.Core.Application.Commands
{
    public class CoreSystemCommandHandler : CommandHandler,
        IRequestHandler<ComponentChangeCommand, IResponse<bool>>
    {
        #region fields

        protected readonly ILogger<CoreSystemCommandHandler> _Logger = null;
        protected readonly ICoreSystemService _CoreSystemService;
        protected readonly IMessageBus _MessageBus;

        #endregion
        #region ctor

        public CoreSystemCommandHandler(
            ILogger<CoreSystemCommandHandler> logger,
            ICoreSystemService coreSystemService,
            IMessageBus messageBus)
        {
            _Logger = logger;
            _CoreSystemService = coreSystemService;
            _MessageBus = messageBus;
        }

        #endregion

        #region IRequestHandlers

        public async Task<IResponse<bool>> Handle(ComponentChangeCommand command, CancellationToken cancellationToken)
        {
            if (!command.IsValid())
            {
                _Logger.LogError($"Componnet validation for [{command.Component.Name}] failed.");
                return command.ValidationResult;
            }

            var result = _CoreSystemService.AddComponent(command.Component);
            if (!result)
            {
                _Logger.LogError($"Componnet [{command.Component.Name}] could not be updated.");

                command.ValidationResult.Payload = false;
                command.ValidationResult.Result = ResponseResultEnumeration.Error;
                return command.ValidationResult;
            }

            var componentsUpdatedEvent = new ComponentsChangedEvent(_CoreSystemService.Components);
            await _MessageBus.PublishEvent(componentsUpdatedEvent);

            return command.ValidationResult;
        }

        #endregion
    }
}
