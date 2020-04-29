namespace EspIot.Application.Interfaces
{
    public interface IOutboundEventBus
    {
        void Send(IEvent eventMessage);
    }
}
