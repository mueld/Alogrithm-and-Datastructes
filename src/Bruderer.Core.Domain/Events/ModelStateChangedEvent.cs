using Bruderer.Core.Domain.Models.ModelAggregate;
using System;

namespace Bruderer.Core.Domain.Events
{
    public class ModelStateChangedEvent : ModelContextEvent
    {
        #region ctor

        public ModelStateChangedEvent(Guid modelId, ModelStateEnumeration newState, ModelStateEnumeration oldState)
            : base(modelId)
        {
            NewState = newState;
            OldState = oldState;
        }

        #endregion
        #region props

        public ModelStateEnumeration NewState { get; private set; }
        public ModelStateEnumeration OldState { get; private set; }

        #endregion
    }
}
