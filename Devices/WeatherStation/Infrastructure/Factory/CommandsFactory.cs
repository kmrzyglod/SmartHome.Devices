using System;
using System.Collections;
using EspIot.Core.Messaging.Interfaces;
using WeatherStation.Application.Commands.Ping;
using WeatherStation.Application.Commands.SetTelemetryInterval;
using WeatherStation.Application.Commands.StartTelemetryService;
using WeatherStation.Application.Commands.StopTelemetryService;

namespace WeatherStation.Infrastructure.Factory
{
    public class CommandsFactory : ICommandsFactory
    {
        private static Hashtable _mappings { get; } = new Hashtable
        {
            //Add here all command factories
            {nameof(PingCommand), new PingCommand.Factory()},
            {nameof(SetTelemetryIntervalCommand), new SetTelemetryIntervalCommand.Factory()},
            {nameof(StartTelemetryServiceCommand), new StartTelemetryServiceCommand.Factory()},
            {nameof(StopTelemetryServiceCommand), new StopTelemetryServiceCommand.Factory()}

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