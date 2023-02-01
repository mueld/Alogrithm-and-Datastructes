using Bruderer.Core.Domain.Messaging;
using Bruderer.Core.Domain.Messaging.Response;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Commands
{
    public class ModelVariablesValueChangeCommand : ModelCommand
    {
        #region ctor

        public ModelVariablesValueChangeCommand(Guid modelId, IEnumerable<IModelVariableDEO> modelVariableDEOs, Guid callerId, string callerName)
            : base(modelId)
        {
            CallerContext.Id = callerId;
            CallerContext.Name = callerName;
            ModelVariableDEOs = new List<IModelVariableDEO>(modelVariableDEOs);
            IgnoredContextIds = new List<Guid>();
        }

        public ModelVariablesValueChangeCommand(Guid modelId, IEnumerable<IModelVariableDEO> modelVariableDEOs, Guid callerId, string callerName, IEnumerable<Guid> ignoredContextIds) 
            : this(modelId, modelVariableDEOs, callerId, callerName)
        {
            IgnoredContextIds = new List<Guid>(ignoredContextIds);
        }

        #endregion
        #region props

        public List<IModelVariableDEO> ModelVariableDEOs { get; protected set; }
        /// <summary>
        /// List with context IDs, which will be ignored for the command processing.
        /// </summary>
        public List<Guid> IgnoredContextIds { get; protected set; }

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
