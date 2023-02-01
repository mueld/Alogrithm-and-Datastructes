using Bruderer.Core.Domain.Events;
using Bruderer.Core.Domain.Interfaces;
using Bruderer.Core.Domain.Messaging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bruderer.Core.Infrastructure.Data.EventSourcing
{
    public class MemoryEventStore : IEventStore
    {
        #region ctor

        public MemoryEventStore()
        {

        }

        #endregion
        #region props

        public List<StoredEvent> EventHistory { get; private set; } = new List<StoredEvent>();
        public uint HistoryOverflow { get; set; } = 10000;

        #endregion
        #region methods

        private void AddEvent(StoredEvent eventMessage)
        {
            EventHistory.Add(eventMessage);

            if (EventHistory.Count > HistoryOverflow)
                EventHistory.RemoveAt(0);
        }

        #endregion

        #region IEventStore

        public void Save<T>(T eventMessage)
            where T : Event
        {
            // Serialized event
            var eventData = JsonConvert.SerializeObject(eventMessage);

            AddEvent(new StoredEvent(eventMessage, eventData));
        }

        #endregion
    }
}
