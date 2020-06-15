using System;
using EspIot.Application.Interfaces;

namespace GreenhouseController.Application.Events.Outbound
{
    public class DoorOpenedEvent : IEvent
    {
        public DoorOpenedEvent()
        {
            CorrelationId = Guid.NewGuid().ToString();
        }

        public string CorrelationId { get; }
    }
}
