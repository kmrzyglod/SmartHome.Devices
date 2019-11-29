using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;
using Infrastructure.Commands.Command;
using Infrastructure.Services.WindowsManager;

namespace Infrastructure.Commands.Handler
{
    public class CloseWindowCommandHandler: ICommandHandler
    {
        private readonly WindowsManagerService _windowsManagerServices;
        private readonly CommandResultEventHandler _commandResultEventHandler;

        public CloseWindowCommandHandler(WindowsManagerService windowsManagerService, CommandResultEventHandler commandResultEventHandler)
        {
            _windowsManagerServices = windowsManagerService;
            _commandResultEventHandler = commandResultEventHandler;
        }

        private void Handle(CloseWindowCommand command)
        {
            _windowsManagerServices.CloseWindows(command.WindowIds,
                (sender) =>
                {
                    _commandResultEventHandler(this, new CommandResultEvent(command.CorrelationId, StatusCode.Success));
                },
                (sender, e) =>
                {
                    _commandResultEventHandler(this, new CommandResultEvent(command.CorrelationId, e.Status, e.ErrorMessage));
                });
        }

        public void Handle(object command)
        {
            Handle(command as CloseWindowCommand);
        }
    }
}
