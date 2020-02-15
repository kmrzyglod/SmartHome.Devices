using System;
using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Interfaces;

namespace EspIot.Core.Messaging.Concrete
{
    public class DeviceStatusUpdatedEvent: IEvent
    {
        public DeviceStatusUpdatedEvent(string message, DeviceStatusCode deviceStatusCode)
        {
            Message = message;
            DeviceStatusCode = deviceStatusCode;
        }

        public string CorrelationId { get; } = Guid.NewGuid().ToString();
        public string Message { get; }
        public DeviceStatusCode DeviceStatusCode { get; }
    }
}
