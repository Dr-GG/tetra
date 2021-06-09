using Tetra.Domain.Enums;

namespace Tetra.Domain.Notifications
{
    public record NotificationEvent
    {
        public NotificationEvent(NotificationEventType type)
        {
            Type = type;
        }

        public NotificationEvent(NotificationEventType type, long value)
        {
            Value = value;
            Type = type;
        }

        public long Value { get; }
        public NotificationEventType Type { get; set; }
    }
}
