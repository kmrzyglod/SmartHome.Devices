using EspIot.Application.Interfaces;

namespace WindowsController.Application.Events.Outbound
{
    public class WindowOpenedEvent: IEvent
    {
        public WindowOpenedEvent(ushort windowId)
        {
            WindowId = windowId;
        }

        public ushort WindowId { get; }
    }
}
