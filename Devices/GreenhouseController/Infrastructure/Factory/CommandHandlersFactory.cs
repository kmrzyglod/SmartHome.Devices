using System;
using System.Collections;
using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;
using GreenhouseController.Application.Commands.CloseWindow;
using GreenhouseController.Application.Commands.OpenWindow;
using GreenhouseController.Application.Services.WindowsManager;

namespace Infrastructure.Factory
{
    class CommandHandlersFactory: ICommandHandlersFactory
    {
        private static Hashtable _mappings { get; set; }

        public CommandHandlersFactory(WindowsManagerService windowsManagerService, IOutboundEventBus outboundEventBus)
        {
            _mappings = new Hashtable
            {
                //Add here all command factories
                {nameof(CloseWindowCommand), new CloseWindowCommandHandler(windowsManagerService, outboundEventBus)},
                {nameof(OpenWindowCommand), new OpenWindowCommandHandler(windowsManagerService, outboundEventBus)}
            };
        }

        public ICommandHandler Get(string commandName)
        {
            if (!_mappings.Contains(commandName))
            {
                throw new NotSupportedException($"Command handler for command {commandName} not found");
            }

            return _mappings[commandName] as ICommandHandler;
        }
    }
}
