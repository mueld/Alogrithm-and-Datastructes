using System;

namespace Bruderer.Core.Domain.Interfaces
{
    public interface IModelEvent : IEvent
    {
        Guid ModelId { get; }
        string ModelName { get; set; }
    }
}
