using Bruderer.Core.Domain.Messaging.Notification;
using Bruderer.Core.Domain.Models.ValueObjects;

namespace Bruderer.Core.Domain.Interfaces
{
    public interface ITextNotification : IEvent
    {
        LocalizableValue Display { get; set; }
        NotificationTypeEnumeration NotificationType { get; set; }
        NotificationAcknowledgeTypeEnumeration AcknowledgeType { get; set; }
    }
}
