using System;
using System.Collections;
using WindowsController.Application.Commands.CloseWindow;
using WindowsController.Application.Commands.OpenWindow;
using WindowsController.Application.Services;
using EspIot.Application.Commands.Ping;
using EspIot.Application.Commands.SendDiagnosticData;
using EspIot.Application.Interfaces;
using EspIot.Application.Services;

namespace WindowsController.Infrastructure.Factory
{
    public class CommandHandlersFactory : ICommandHandlersFactory
    {
        public CommandHandlersFactory(IOutboundEventBus outboundEventBus, IDiagnosticService diagnosticService,
            WindowsManagingService windowsManagingService)
        {
            _mappings = new Hashtable
            {
                //Add here all command factories
                {nameof(PingCommand), new PingCommandHandler(outboundEventBus)},
                {nameof(SendDiagnosticDataCommand), new SendDiagnosticDataCommandHandler(outboundEventBus, diagnosticService)},
                {nameof(OpenWindowCommand), new OpenWindowCommandHandler(outboundEventBus, windowsManagingService)},
                {nameof(CloseWindowCommand), new CloseWindowCommandHandler(outboundEventBus, windowsManagingService)}
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