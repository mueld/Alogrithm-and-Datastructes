using System;

namespace Bruderer.Core.Domain.Interfaces
{
    public interface IModelUserEvent : IEvent
    {
        Guid ModelUserId { get; }
        string ModelUserName { get; set; }
    }
}
