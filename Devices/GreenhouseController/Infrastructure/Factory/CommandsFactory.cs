using System;
using System.Collections;
using EspIot.Application.Commands.Ping;
using EspIot.Application.Commands.SendDiagnosticData;
using EspIot.Application.Interfaces;

namespace Infrastructure.Factory
{
    public class CommandsFactory : ICommandsFactory
    {
        private static Hashtable _mappings { get; } = new Hashtable
        {
            //Add here all commands 
            {nameof(PingCommand), new PingCommand.Factory()},
            {nameof(SendDiagnosticDataCommand), new SendDiagnosticDataCommand.Factory()}
        };

        public ICommand Create(string commandName, Hashtable payload)
        {
            if (!_mappings.Contains(commandName))
            {
                throw new NotSupportedException($"Command {commandName} is not supported");
            }

            return (_mappings[commandName] as ICommandFactory)?.Create(payload);
        }
    }
}