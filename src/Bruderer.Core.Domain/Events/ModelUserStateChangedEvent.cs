using Bruderer.Core.Domain.Messaging;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using System;

namespace Bruderer.Core.Domain.Events
{
    public class ModelUserStateChangedEvent : ModelUserEvent
    {
        #region ctor

        public ModelUserStateChangedEvent(Guid modelUserId, ModelUserStateEnumeration newState, ModelUserStateEnumeration oldState)
            : base(modelUserId)
        {
            NewState = newState;
            OldState = oldState;
        }

        #endregion
        #region props

        public ModelUserStateEnumeration NewState { get; private set; }
        public ModelUserStateEnumeration OldState { get; private set; }

        #endregion
    }
}
