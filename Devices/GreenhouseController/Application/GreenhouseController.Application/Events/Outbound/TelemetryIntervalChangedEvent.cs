using System;
using EspIot.Application.Events;

namespace GreenhouseController.Application.Events.Outbound
{
    public class TelemetryIntervalChangedEvent: EventBase
    {
        public TelemetryIntervalChangedEvent(int newInterval)
        {
            NewInterval = newInterval;
        }

        public string CorrelationId { get; } = Guid.NewGuid().ToString();
        public int NewInterval { get; }
    }
}
