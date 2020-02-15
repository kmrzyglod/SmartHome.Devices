using System.Collections;
using EspIot.Core.Extensions;
using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Interfaces;
using WeatherStation.Application.Services;

namespace WeatherStation.Application.Commands.StopTelemetryService
{
    public class StopTelemetryServiceCommand : CommandBase
    {
        public StopTelemetryServiceCommand(string correlationId) : base(correlationId)
        {
        }

        public override string PartitionKey { get; } = nameof(TelemetryService);

        public static StopTelemetryServiceCommand FromHashtable(Hashtable obj)
        {
            return new StopTelemetryServiceCommand(obj.GetString(nameof(CorrelationId)));
        }

        public class Factory : ICommandFactory
        {
            ICommand ICommandFactory.Create(Hashtable obj)
            {
                return FromHashtable(obj);
            }
        }
    }
}