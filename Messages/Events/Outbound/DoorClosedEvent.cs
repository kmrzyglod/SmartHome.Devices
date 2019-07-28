using EspIot.Core.Messaging.Interfaces;

namespace Infrastructure.Events.Outbound
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
