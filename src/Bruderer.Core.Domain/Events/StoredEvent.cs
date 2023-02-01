using Bruderer.Core.Domain.Messaging;

namespace Bruderer.Core.Domain.Events
{
    public class StoredEvent : Event
    {
        #region ctor

        public StoredEvent(Event eventMessage, string data)
        {
            MessageType = eventMessage.MessageType;
            Data = data;
        }

        #endregion
        #region props

        public string Data { get; private set; }

        #endregion
    }
}
