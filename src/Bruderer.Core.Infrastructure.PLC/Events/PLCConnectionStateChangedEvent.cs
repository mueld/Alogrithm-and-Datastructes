using Bruderer.Core.Domain.Messaging;
using Bruderer.Core.Infrastructure.PLC.Connection;
using System;

namespace Bruderer.Core.Infrastructure.PLC.Events
{
    public class PLCConnectionStateChangedEvent : ModelUserEvent
    {
        #region ctor

        public PLCConnectionStateChangedEvent(
            Guid modelUserId,
            PLCConnectionStateEnumeration newState,
            PLCConnectionStateEnumeration oldState)
            : base(modelUserId)
        {
            NewState = newState;
            OldState = oldState;
        }

        #endregion
        #region props

        public PLCConnectionStateEnumeration NewState { get; private set; }
        public PLCConnectionStateEnumeration OldState { get; private set; }

        #endregion
    }
}
