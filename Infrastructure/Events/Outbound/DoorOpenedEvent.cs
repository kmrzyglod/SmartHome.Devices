using System;
using EspIot.Core.Messaging.Interfaces;

namespace Infrastructure.Events.Outbound
{
    public class DoorOpenedEvent : IMessage
    {
        public DoorOpenedEvent()
        {
            CorrelationId = Guid.NewGuid().ToString();
        }

        public string CorrelationId { get; }
    }
}
