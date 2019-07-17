using EspIot.Core.Messaging.Interfaces;
using System;

namespace Messages.Events.Outbound
{
    public class WindowClosedEvent : IMessage
    {
        public WindowClosedEvent(ushort windowNum)
        {
            CorrelationId = new Guid().ToString();
            WindowNum = windowNum;
        }
        
        public string CorrelationId { get; }
        public ushort WindowNum { get; }
    }
}
