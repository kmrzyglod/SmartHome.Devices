using EspIot.Application.Interfaces;

namespace WindowsController.Application.Events.Outbound
{
    public class WindowsControllerTelemetryEvent: IEvent
    {
        public WindowsControllerTelemetryEvent(bool[] windowsStatus)
        {
            WindowsStatus = windowsStatus;
        }

        public bool[] WindowsStatus { get; } 
    }
}
