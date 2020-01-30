using System;
using EspIot.Core.Messaging.Interfaces;

namespace GreenhouseController.Application.Events.Outbound
{
    public class DoorClosedEvent : IMessage
    {
        public DoorClosedEvent()
        {
            CorrelationId = Guid.NewGuid().ToString();
        }
        
        public string CorrelationId { get; }
    }
}
