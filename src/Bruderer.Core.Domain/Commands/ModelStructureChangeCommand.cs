using Bruderer.Core.Domain.Messaging;
using Bruderer.Core.Domain.Messaging.Response;
using Bruderer.Core.Domain.Models.ModelAggregate;
using System;

namespace Bruderer.Core.Domain.Commands
{
    public class ModelStructureChangeCommand : ModelCommand
    {
        #region ctor

        public ModelStructureChangeCommand(Guid modelId, ModelUpdateOptions modelUpdateOptions, Guid callerId, string callerName)
            : base(modelId)
        {
            CallerContext.Id = callerId;
            CallerContext.Name = callerName;
            ModelUpdateOptions = modelUpdateOptions;
        }

        #endregion
        #region props

        public ModelUpdateOptions ModelUpdateOptions { get; protected set; }

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
