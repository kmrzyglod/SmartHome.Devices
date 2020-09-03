using EspIot.Application.Events;

namespace WindowsController.Application.Events.Outbound
{
    public class WindowsControllerTelemetryEvent: EventBase
    {
        public WindowsControllerTelemetryEvent(bool[] windowsStatus)
        {
            WindowsStatus = windowsStatus;
        }

        public bool[] WindowsStatus { get; } 
    }
}
