using System;
using System.Collections;
using EspIot.Application.Commands.Ping;
using EspIot.Application.Commands.SendDiagnosticData;
using EspIot.Application.Interfaces;
using EspIot.Application.Services;

namespace Infrastructure.Factory
{
    public class CommandHandlersFactory : ICommandHandlersFactory
    {
        public CommandHandlersFactory(IOutboundEventBus outboundEventBus, IDiagnosticService diagnosticService)
        {
            _mappings = new Hashtable
            {
                //Add here all command factories
                {nameof(PingCommand), new PingCommandHandler(outboundEventBus)},
                {
                    nameof(SendDiagnosticDataCommand),
                    new SendDiagnosticDataCommandHandler(outboundEventBus, diagnosticService)
                }
            };
        }

        private static Hashtable _mappings { get; set; }

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