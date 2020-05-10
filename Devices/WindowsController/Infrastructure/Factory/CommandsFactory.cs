using System;
using System.Collections;
using WindowsController.Application.Commands.CloseWindow;
using WindowsController.Application.Commands.OpenWindow;
using EspIot.Application.Commands.Ping;
using EspIot.Application.Commands.SendDiagnosticData;
using EspIot.Application.Interfaces;

namespace WindowsController.Infrastructure.Factory
{
    public class CommandsFactory : ICommandsFactory
    {
        private static Hashtable _mappings { get; } = new Hashtable
        {
            //Add here all commands 
            {nameof(PingCommand), new PingCommand.Factory()},
            {nameof(SendDiagnosticDataCommand), new SendDiagnosticDataCommand.Factory()},
            {nameof(OpenWindowCommand), new OpenWindowCommand.Factory()},
            {nameof(CloseWindowCommand), new CloseWindowCommand.Factory()}
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