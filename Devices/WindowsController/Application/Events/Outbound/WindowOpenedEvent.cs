using EspIot.Application.Events;
using EspIot.Application.Interfaces;

namespace WindowsController.Application.Events.Outbound
{
    public class WindowOpenedEvent: EventBase
    {
        public WindowOpenedEvent(ushort windowId)
        {
            WindowId = windowId;
        }

        public ushort WindowId { get; }
    }
}
