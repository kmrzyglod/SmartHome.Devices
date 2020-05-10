using EspIot.Application.Interfaces;

namespace WindowsController.Application.Events.Outbound
{
    public class WindowClosedEvent: IEvent
    {
        public WindowClosedEvent(ushort windowId)
        {
            WindowId = windowId;
        }

        public ushort WindowId { get; }
    }
}
