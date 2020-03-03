using System.Collections;
using EspIot.Core.Extensions;
using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Interfaces;
using EspIot.Core.Messaging.Validation;
using WeatherStation.Application.Services;

namespace WeatherStation.Application.Commands.SetTelemetryInterval
{
    public class SetTelemetryIntervalCommand : CommandBase
    {
        public  const int MAX_INTERVAL = 3_600_000;
        public const int MIN_INTERVAL = 30_000;

        public SetTelemetryIntervalCommand(string correlationId, int interval) : base(correlationId)
        {
            Interval = interval;
        }

        public override string PartitionKey { get; } = nameof(TelemetryService);
        public int Interval { get; }

        public override ValidationError[] Validate()
        {
            var errors = ValidateBase();

            if (Interval > MAX_INTERVAL)
            {
                errors.Add(new ValidationError(nameof(Interval),
                    $"{nameof(Interval)} field cannot has value higher than {MAX_INTERVAL}"));
            }

            else if (Interval < MIN_INTERVAL)
            {
                errors.Add(new ValidationError(nameof(Interval),
                    $"{nameof(Interval)} field cannot has value less than {MIN_INTERVAL}"));
            }

            return ConvertToArray(errors);
        }

        public static SetTelemetryIntervalCommand FromHashtable(Hashtable obj)
        {
            var command = new SetTelemetryIntervalCommand(obj.GetString(nameof(CorrelationId)), obj.GetInt(nameof(Interval)));
            return command;
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