using System;

namespace Bruderer.Core.Domain.Messaging
{
    public abstract class Message
    {
        #region ctor

        protected Message()
        {
            MessageType = GetType().Name;
        }

        #endregion
        #region props

        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public string MessageType { get; protected set; } = string.Empty;
        public string MessageInfo { get; set; } = string.Empty;
        public CallerContext CallerContext { get; set; } = new CallerContext();

        #endregion
    }
}
