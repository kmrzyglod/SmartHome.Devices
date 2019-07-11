using EspIot.Core.Messaging.Interfaces;

namespace Messages.Events.Outbound
{
    public class DoorClosedEvent : IMessage
    {
        public string CorrelationId { get; }
    }
}
