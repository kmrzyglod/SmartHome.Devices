using System;
using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Interfaces;

namespace EspIot.Application.Events.Outbound
{
    public class DeviceStatusUpdatedEvent: IEvent
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
