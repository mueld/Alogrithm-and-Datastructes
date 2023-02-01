using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Events
{
    public class ModelStructureChangedEvent : ModelContextEvent
    {
        #region ctor

        public ModelStructureChangedEvent(Guid modelId, List<string> serviceNames)
            : base(modelId)
        {
            ServiceNames = serviceNames;
        }

        #endregion
        #region props

        public List<string> ServiceNames { get; protected set; } = new();

        #endregion
    }
}
