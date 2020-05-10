using WindowsController.Application.Services;
using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;
using EspIot.Core.Messaging.Enum;

namespace WindowsController.Application.Commands.OpenWindow
{
    public class OpenWindowCommandHandler: ICommandHandler
    {
        private readonly IOutboundEventBus _outboundEventBus;
        private readonly WindowsManagingService _windowsManagingService;

        public OpenWindowCommandHandler(IOutboundEventBus outboundEventBus,
            WindowsManagingService windowsManagingService)
        {
            _outboundEventBus = outboundEventBus;
            _windowsManagingService = windowsManagingService;
        }

        public void Handle(ICommand command)
        {
            Handle(command as OpenWindowCommand);
        }

        private void Handle(OpenWindowCommand command)
        {
            _windowsManagingService.OpenWindow(command.WindowId);
            _outboundEventBus.Send(new CommandResultEvent(command.CorrelationId, StatusCode.Success,
                nameof(OpenWindowCommand)));
        }
    }
}
