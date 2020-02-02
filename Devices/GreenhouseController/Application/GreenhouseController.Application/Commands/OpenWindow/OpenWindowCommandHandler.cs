using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;
using GreenhouseController.Application.Services.WindowsManager;

namespace GreenhouseController.Application.Commands.OpenWindow
{
    public class OpenWindowCommandHandler : ICommandHandler
    {
        private readonly IOutboundEventBus _outboundEventBus;
        private readonly WindowsManagerService _windowsManagerServices;

        public OpenWindowCommandHandler(WindowsManagerService windowsManagerServices,
            IOutboundEventBus outboundEventBus)
        {
            _windowsManagerServices = windowsManagerServices;
            _outboundEventBus = outboundEventBus;
        }

        public void Handle(ICommand command)
        {
            Handle(command as OpenWindowCommand);
        }

        private void Handle(OpenWindowCommand command)
        {
            _windowsManagerServices.OpenWindows(command.WindowIds,
                sender =>
                {
                    _outboundEventBus.Send(new CommandResultEvent(command.CorrelationId, StatusCode.Success));
                },
                (sender, e) =>
                {
                    _outboundEventBus.Send(
                        new CommandResultEvent(command.CorrelationId, e.Status, e.ErrorMessage));
                });
        }
    }
}