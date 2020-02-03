using System;
using System.Collections;
using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;

namespace WeatherStation.Infrastructure
{
    class CommandHandlersFactory: ICommandHandlersFactory
    {
        private static Hashtable _mappings { get; set; }

        public CommandHandlersFactory()
        {
            _mappings = new Hashtable
            {
                //Add here all command factories
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
