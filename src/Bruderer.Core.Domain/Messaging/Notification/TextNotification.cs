using Bruderer.Core.Domain.Interfaces;
using Bruderer.Core.Domain.Models.ValueObjects;

namespace Bruderer.Core.Domain.Messaging.Notification
{
    public class TextNotification : Event, ITextNotification
    {
        #region ctor

        public TextNotification()
        {

        }

        #endregion

        #region ITextNotification

        public LocalizableValue Display { get; set; } = new LocalizableValue();
        public NotificationTypeEnumeration NotificationType { get; set; } = NotificationTypeEnumeration.Unknown;
        public NotificationAcknowledgeTypeEnumeration AcknowledgeType { get; set; } = NotificationAcknowledgeTypeEnumeration.Unknown;

        #endregion
    }
}
