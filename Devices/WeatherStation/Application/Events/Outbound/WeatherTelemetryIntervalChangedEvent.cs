using System;
using EspIot.Application.Events;
using EspIot.Application.Interfaces;

namespace WeatherStation.Application.Events.Outbound
{
    public class WeatherTelemetryIntervalChangedEvent: EventBase
    {
        public WeatherTelemetryIntervalChangedEvent(int newInterval)
        {
            NewInterval = newInterval;
        }

        public string CorrelationId { get; } = Guid.NewGuid().ToString();
        public int NewInterval { get; }
    }
}
