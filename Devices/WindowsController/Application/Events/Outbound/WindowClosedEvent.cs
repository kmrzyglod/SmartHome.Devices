using EspIot.Application.Events;

namespace WindowsController.Application.Events.Outbound
{
    public class WindowClosedEvent: EventBase
    {
        public WindowClosedEvent(ushort windowId)
        {
            WindowId = windowId;
        }

        public ushort WindowId { get; }
    }
}
