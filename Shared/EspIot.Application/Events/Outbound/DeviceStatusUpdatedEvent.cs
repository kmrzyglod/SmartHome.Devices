using System;
using EspIot.Application.Interfaces;
using EspIot.Core.Messaging.Enum;

namespace EspIot.Application.Events.Outbound
{
    public class DeviceStatusUpdatedEvent: EventBase
    {
        public DeviceStatusUpdatedEvent(string message, DeviceStatusCode deviceStatusCode)
        {
            Message = message;
            DeviceStatusCode = deviceStatusCode;
        }

        public string Message { get; }
        public DeviceStatusCode DeviceStatusCode { get; }
    }
}
