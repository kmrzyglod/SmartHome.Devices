using System;
using EspIot.Application.Interfaces;

namespace GreenhouseController.Application.Events.Outbound
{
    public class DoorClosedEvent : IEvent
    {
        public DoorClosedEvent()
        {
            CorrelationId = Guid.NewGuid().ToString();
        }
        
        public string CorrelationId { get; }
    }
}
