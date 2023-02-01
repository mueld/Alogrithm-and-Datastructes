using Bruderer.Core.Domain.Messaging;
using Bruderer.Core.Infrastructure.PLC.Connection;
using System;

namespace Bruderer.Core.Infrastructure.PLC.Events
{
    public class PLCRuntimeStateChangedEvent : ModelUserEvent
    {
        #region ctor

        public PLCRuntimeStateChangedEvent(
            Guid modelUserId,
            PLCRuntimeStateEnumeration newState,
            PLCRuntimeStateEnumeration oldState)
            : base(modelUserId)
        {
            NewState = newState;
            OldState = oldState;
        }

        #endregion
        #region props

        public PLCRuntimeStateEnumeration NewState { get; private set; }
        public PLCRuntimeStateEnumeration OldState { get; private set; }

        #endregion
    }
}
