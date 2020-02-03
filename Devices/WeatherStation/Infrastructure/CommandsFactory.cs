using System;
using System.Collections;
using EspIot.Core.Messaging.Interfaces;

namespace WeatherStation.Infrastructure
{
    class CommandsFactory: ICommandsFactory
    {
        private static Hashtable _mappings { get; } = new Hashtable
        {
            //Add here all command factories
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
