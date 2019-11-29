using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;
using Infrastructure.Commands.Command;
using Infrastructure.Services.WindowsManager;

namespace Infrastructure.Commands.Handler
{
    public class OpenWindowCommandHandler : ICommandHandler
    {
        private readonly CommandResultEventHandler _commandResultEventHandler;
        private readonly WindowsManagerService _windowsManagerServices;

        public OpenWindowCommandHandler(WindowsManagerService windowsManagerServices,
            CommandResultEventHandler commandResultEventHandler)
        {
            _windowsManagerServices = windowsManagerServices;
            _commandResultEventHandler = commandResultEventHandler;
        }

        public void Handle(object command)
        {
            Handle(command as OpenWindowCommand);
        }

        private void Handle(OpenWindowCommand command)
        {
            _windowsManagerServices.OpenWindows(command.WindowIds,
                sender =>
                {
                    _commandResultEventHandler(this, new CommandResultEvent(command.CorrelationId, StatusCode.Success));
                },
                (sender, e) =>
                {
                    _commandResultEventHandler(this,
                        new CommandResultEvent(command.CorrelationId, e.Status, e.ErrorMessage));
                });
        }
    }
}