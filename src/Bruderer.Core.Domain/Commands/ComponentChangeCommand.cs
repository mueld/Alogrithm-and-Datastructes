using Bruderer.Core.Domain.Messaging;
using Bruderer.Core.Domain.Messaging.Response;
using Bruderer.Core.Domain.Models.ComponentAggregate;

namespace Bruderer.Core.Domain.Commands
{
    public class ComponentChangeCommand : Command
    {
        #region ctor

        public ComponentChangeCommand(IComponent component)
        {
            Component = component;
        }

        #endregion
        #region props

        public IComponent Component { get; private set; }

        #endregion
        #region methods

        public override bool IsValid()
        {
            ValidationResult.Payload = true;
            ValidationResult.Result = ResponseResultEnumeration.Success;
            return ValidationResult.Result == ResponseResultEnumeration.Success;
        }

        #endregion
    }
}
