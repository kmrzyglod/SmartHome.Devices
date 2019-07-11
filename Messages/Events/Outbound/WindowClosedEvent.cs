using EspIot.Core.Messaging.Interfaces;
using System;

namespace Messages.Events.Outbound
{
    public class WindowClosedEvent : IMessage
    {
        public string CorrelationId { get; }
    }
}
