using Bruderer.Core.Domain.Interfaces;
using System;

namespace Bruderer.Core.Domain.Messaging
{
    public abstract class ModelUserEvent : Event, IModelUserEvent
    {
        #region ctor

        protected ModelUserEvent(Guid modelId)
        {
            ModelUserId = modelId;
        }

        #endregion
        #region props

        public Guid ModelUserId { get; protected set; } = Guid.Empty;
        public string ModelUserName { get; set; } = string.Empty;

        #endregion
    }
}
