using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Events
{
    public class ModelVariablesMetadataChangedEvent : ModelContextEvent
    {
        #region ctor

        public ModelVariablesMetadataChangedEvent(Guid modelId, List<Guid> variableIDs)
            : base(modelId)
        {
            VariableIDs = variableIDs;
        }

        #endregion
        #region props

        public List<Guid> VariableIDs { get; protected set; } = new List<Guid>();

        #endregion
    }
}
