using EspIot.Core.Messaging.Interfaces;

namespace EspIot.Core.Messaging.Events
{
    public interface IOutboundEventBus
    {
        void Send(IEvent eventMessage);
    }
}
