using System;
using System.Collections;
using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;
using WeatherStation.Application.Commands.Ping;
using WeatherStation.Application.Commands.SetTelemetryInterval;
using WeatherStation.Application.Commands.StartTelemetryService;
using WeatherStation.Application.Commands.StopTelemetryService;
using WeatherStation.Application.Services;

namespace WeatherStation.Infrastructure.Factory
{
    public class CommandHandlersFactory : ICommandHandlersFactory
    {
        public CommandHandlersFactory(IOutboundEventBus outboundEventBus, TelemetryService telemetryService)
        {
            _mappings = new Hashtable
            {
                //Add here all command factories
                {nameof(PingCommand), new PingCommandHandler(outboundEventBus)},
                {nameof(SetTelemetryIntervalCommand), new SetTelemetryIntervalCommandHandler(telemetryService, outboundEventBus)},
                {nameof(StartTelemetryServiceCommand), new StartTelemetryServiceCommandHandler(telemetryService, outboundEventBus)},
                {nameof(StopTelemetryServiceCommand), new StopTelemetryServiceCommandHandler(telemetryService, outboundEventBus)}
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