using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Events
{
    public class ModelVariablesValueChangedEvent : ModelContextEvent
    {
        #region ctor

        public ModelVariablesValueChangedEvent(Guid modelId, List<Guid> variableIDs)
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
