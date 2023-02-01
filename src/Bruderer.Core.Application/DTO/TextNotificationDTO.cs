using Bruderer.Core.Domain.Messaging.Notification;
using Bruderer.Core.Domain.Models.ValueObjects;

namespace Bruderer.Core.Application.DTO
{
    // OFI -> Check for auto mapping solutions as https://github.com/AutoMapper/AutoMapper
    public class TextNotificationDTO
    {
        #region ctor

        public TextNotificationDTO()
        { }

        public TextNotificationDTO(TextNotification textNotification)
        {
            Display = textNotification.Display;
            NotificationType = textNotification.NotificationType;
            AcknowledgeType = textNotification.AcknowledgeType;
        }

        #endregion
        #region props

        public LocalizableValue Display { get; set; } = new LocalizableValue();
        public NotificationTypeEnumeration NotificationType { get; set; } = NotificationTypeEnumeration.Unknown;
        public NotificationAcknowledgeTypeEnumeration AcknowledgeType { get; set; } = NotificationAcknowledgeTypeEnumeration.Unknown;

        #endregion
    }
}
