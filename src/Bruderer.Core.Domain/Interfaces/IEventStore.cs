using Bruderer.Core.Domain.Messaging;

namespace Bruderer.Core.Domain.Interfaces
{
    public interface IEventStore
    {
        void Save<T>(T eventMessage) where T : Event;
    }
}
