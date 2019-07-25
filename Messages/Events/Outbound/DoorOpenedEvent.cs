using EspIot.Core.Messaging.Interfaces;

namespace Messages.Events.Outbound
{
    public class DoorOpenedEvent : IMessage
    {
        public DoorOpenedEvent()
        {
            CorrelationId = new System.Guid().ToString();
        }

        public string CorrelationId { get; }
    }
}
