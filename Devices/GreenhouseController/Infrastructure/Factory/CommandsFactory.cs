using System;
using System.Collections;
using EspIot.Core.Messaging.Interfaces;
using GreenhouseController.Application.Commands.CloseWindow;
using GreenhouseController.Application.Commands.OpenWindow;

namespace Infrastructure.Factory
{
    public class CommandsFactory: ICommandsFactory
    {
        private static Hashtable _mappings { get; } = new Hashtable
        {
            //Add here all command factories
            { nameof(CloseWindowCommand), new CloseWindowCommand.Factory() },
            { nameof(OpenWindowCommand), new OpenWindowCommand.Factory() }
        };

        public ICommand Create(string commandName, Hashtable payload)
        {
            if (!_mappings.Contains(commandName))
            {
                throw new NotSupportedException($"Command {commandName} is not supported");
            }

            return _mappings[commandName] as ICommand;
        }
    }
}
