using Bruderer.Core.Domain.Interfaces;
using System;

namespace Bruderer.Core.Domain.Messaging
{
    public abstract class ModelEvent : Event, IModelEvent
    {
        #region ctor

        protected ModelEvent(Guid modelId)
        {
            ModelId = modelId;
        }

        #endregion
        #region props

        public Guid ModelId { get; protected set; } = Guid.Empty;
        public string ModelName { get; set; } = string.Empty;

        #endregion
    }
}
