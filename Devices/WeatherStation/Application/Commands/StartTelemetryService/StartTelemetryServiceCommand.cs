using System.Collections;
using EspIot.Application.Commands;
using EspIot.Application.Interfaces;
using EspIot.Core.Extensions;
using WeatherStation.Application.Services;

namespace WeatherStation.Application.Commands.StartTelemetryService
{
    public class StartTelemetryServiceCommand: CommandBase
    {
        public StartTelemetryServiceCommand(string correlationId) : base(correlationId)
        {
        }

        public override string PartitionKey { get; } = nameof(TelemetryService);
        
        public static StartTelemetryServiceCommand FromHashtable(Hashtable obj)
        {
            return new StartTelemetryServiceCommand(obj.GetString(nameof(CorrelationId)));
        }
        
        public class Factory: ICommandFactory
        {
            ICommand ICommandFactory.Create(Hashtable obj)
            {
                return FromHashtable(obj);
            }
        }
    }
}
