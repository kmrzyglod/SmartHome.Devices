using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;
using GreenhouseController.Application.Services.WindowsManager;

namespace GreenhouseController.Application.Commands.CloseWindow
{
    public class CloseWindowCommandHandler : ICommandHandler
    {
        private readonly IOutboundEventBus _outboundEventBus;
        private readonly WindowsManagerService _windowsManagerServices;

        public CloseWindowCommandHandler(WindowsManagerService windowsManagerService,
            IOutboundEventBus outboundEventBus)
        {
            _windowsManagerServices = windowsManagerService;
            _outboundEventBus = outboundEventBus;
        }

        public void Handle(ICommand command)
        {
            Handle(command as CloseWindowCommand);
        }

        private void Handle(CloseWindowCommand command)
        {
            _windowsManagerServices.CloseWindows(command.WindowIds,
                sender =>
                {
                    _outboundEventBus.Send(new CommandResultEvent(command.CorrelationId, StatusCode.Success));
                },
                (sender, e) =>
                {
                    _outboundEventBus.Send(new CommandResultEvent(command.CorrelationId, e.Status, e.ErrorMessage));
                });
        }
    }
}