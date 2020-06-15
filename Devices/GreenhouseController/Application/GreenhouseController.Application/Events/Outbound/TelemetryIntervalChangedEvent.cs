using System;
using EspIot.Application.Interfaces;

namespace GreenhouseController.Application.Events.Outbound
{
    public class TelemetryIntervalChangedEvent: IEvent
    {
        public TelemetryIntervalChangedEvent(int newInterval)
        {
            NewInterval = newInterval;
        }

        public string CorrelationId { get; } = Guid.NewGuid().ToString();
        public int NewInterval { get; }
    }
}
