using EspIot.Core.Messaging.Interfaces;

namespace Messages.Events.Outbound
{
    public class WindowOpenedEvent : IMessage
    {
        public string CorrelationId { get; }
    }
}
