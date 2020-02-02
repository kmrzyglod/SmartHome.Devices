using EspIot.Core.Messaging.Concrete;

namespace GreenhouseController.Application.Events.Internal
{
    public delegate void OnFailureEventHandler(object sender, ProcessingFailureEvent e);
}
