using EspIot.Core.Messaging.Interfaces;

namespace Messages.Events.Outbound
{
    public class DoorOpenedEvent : IMessage
    {
        public string CorrelationId { get; }
    }
}
