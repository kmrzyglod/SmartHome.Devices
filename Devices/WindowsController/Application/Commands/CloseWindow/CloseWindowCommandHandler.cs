using WindowsController.Application.Services;
using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;
using EspIot.Core.Messaging.Enum;

namespace WindowsController.Application.Commands.CloseWindow
{
    public class CloseWindowCommandHandler : ICommandHandler
    {
        private readonly IOutboundEventBus _outboundEventBus;
        private readonly WindowsManagingService _windowsManagingService;

        public CloseWindowCommandHandler(IOutboundEventBus outboundEventBus,
            WindowsManagingService windowsManagingService)
        {
            _outboundEventBus = outboundEventBus;
            _windowsManagingService = windowsManagingService;
        }

        public void Handle(ICommand command)
        {
            Handle(command as CloseWindowCommand);
        }

        private void Handle(CloseWindowCommand command)
        {
            _windowsManagingService.CloseWindow(command.WindowId);
            _outboundEventBus.Send(new CommandResultEvent(command.CorrelationId, StatusCode.Success,
                nameof(CloseWindowCommand)));
        }
    }
}