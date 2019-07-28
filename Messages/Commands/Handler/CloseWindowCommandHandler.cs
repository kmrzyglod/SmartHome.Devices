﻿using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Events;
using Infrastructure.Commands.Command;
using Infrastructure.Services.WindowsManager;

namespace Infrastructure.Commands.Handler
{
    class CloseWindowCommandHandler
    {
        private readonly WindowsManagerService _windowsManagerServices;
        private readonly CommandResultEventHandler _commandResultEventHandler;

        public CloseWindowCommandHandler(WindowsManagerService windowsManagerServices, CommandResultEventHandler commandResultEventHandler)
        {
            _windowsManagerServices = windowsManagerServices;
            _commandResultEventHandler = commandResultEventHandler;
        }

        public void Handle(CloseWindowCommand command)
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
    }
}
