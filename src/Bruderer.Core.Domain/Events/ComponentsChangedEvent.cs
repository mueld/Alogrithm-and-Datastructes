using Bruderer.Core.Domain.Messaging;
using Bruderer.Core.Domain.Models.ComponentAggregate;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Events
{
    public class ComponentsChangedEvent : Event
    {
        #region ctor

        public ComponentsChangedEvent(List<IComponent> components)
        {
            Components = components;
        }

        #endregion
        #region props

        public List<IComponent> Components { get; private set; }

        #endregion
    }
}
