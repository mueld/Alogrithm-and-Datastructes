using Bruderer.Core.Domain.Messaging.Response;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Models.ComponentAggregate
{
    public class Component : Entity, IAggregateRoot, IComponent
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public ComponentStateEnumeration State { get; set; }
        public List<IResponseMessage> MessageStack { get; set; } = new List<IResponseMessage>();
    }
}
