using Bruderer.Core.Domain.Messaging.Response;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Models.ComponentAggregate
{
    public interface IComponent
    {
        Guid Id { get; }
        string Name { get; set; }
        string Version { get; set; }
        ComponentStateEnumeration State { get; set; }
        List<IResponseMessage> MessageStack { get; set; }
    }
}
