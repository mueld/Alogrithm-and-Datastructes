using Bruderer.Core.Domain.Messaging;
using System;

namespace Bruderer.Core.Domain.Events
{
    public abstract class ModelContextEvent : ModelEvent
    {
        protected ModelContextEvent(Guid modelId)
            : base(modelId)
        {
        }
    }
}
