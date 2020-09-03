using System;

namespace EspIot.Application.Interfaces
{
    public interface IEvent
    {
        DateTime Timestamp { get; }
    }
}
