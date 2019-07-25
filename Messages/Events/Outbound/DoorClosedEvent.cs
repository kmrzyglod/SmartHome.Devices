using EspIot.Core.Messaging.Interfaces;

namespace Messages.Events.Outbound
{
    public class DoorClosedEvent : IMessage
    {
        public DoorClosedEvent()
        {
            CorrelationId = new System.Guid().ToString();
        }
        
        public string CorrelationId { get; }
    }
}
