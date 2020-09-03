using System;
using EspIot.Application.Interfaces;

namespace EspIot.Application.Events
{
    public abstract class EventBase: IEvent
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }
}
