using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Events;
using Infrastructure.Commands;
using Infrastructure.Commands.Command;
using Infrastructure.Services.WindowsManager;

namespace Messages.Commands.Handler
{
    class OpenWindowCommandHandler
    {
        private readonly WindowsManagerService _windowsManagerServices;
        private readonly CommandResultEventHandler _commandResultEventHandler;

        public OpenWindowCommandHandler(WindowsManagerService windowsManagerServices, CommandResultEventHandler commandResultEventHandler)
        {
            _windowsManagerServices = windowsManagerServices;
            _commandResultEventHandler = commandResultEventHandler;
        }

        public void Handle(OpenWindowCommand command)
        {
            _windowsManagerServices.OpenWindows(command.WindowIds,
                (sender) =>
                {
                    _commandResultEventHandler(this, new CommandResultEvent(command.CorrelationId, StatusCode.Success));
                },
                (sender, e) =>
                {
                    _commandResultEventHandler(this, new CommandResultEvent(command.CorrelationId, e.Status, e.ErrorMessage));
                });
        }
    }
}
