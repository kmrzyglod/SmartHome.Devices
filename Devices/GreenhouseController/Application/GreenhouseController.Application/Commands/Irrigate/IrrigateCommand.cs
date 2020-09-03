using System.Collections;
using EspIot.Application.Commands;
using EspIot.Application.Interfaces;
using EspIot.Core.Extensions;
using EspIot.Core.Messaging.Validation;
using GreenhouseController.Application.Services.Irrigation;

namespace GreenhouseController.Application.Commands.Irrigate
{
    public class IrrigateCommand : CommandBase
    {
        public const int MAXIMUM_IRRIGATION_TIME = 1200; 
        public const int MAXIMUM_IRRIGATION_VOLUME = 60; 
       
        //Irigation time in seconds
        public int MaximumIrrigationTime { get; }

        //Water volume in liters 
        public int WaterVolume { get; }

        public IrrigateCommand(string correlationId, int maximumIrrigationTime, int waterVolume) : base(correlationId)
        {
            MaximumIrrigationTime = maximumIrrigationTime;
            WaterVolume = waterVolume;
        }

        public static IrrigateCommand FromHashtable(Hashtable obj)
        {
            return new IrrigateCommand(
                obj.GetString(nameof(CorrelationId)),
                obj.GetInt(nameof(MaximumIrrigationTime)),
                obj.GetInt(nameof(WaterVolume)));
        }

        public override string PartitionKey { get; } = nameof(IrrigationService);

        public override ValidationError[] Validate()
        {
            var errors = ValidateBase();
            if (MaximumIrrigationTime > MAXIMUM_IRRIGATION_TIME)
            {
                errors.Add(new ValidationError(nameof(MaximumIrrigationTime),
                    $"{nameof(MaximumIrrigationTime)} field cannot has value higher than {MAXIMUM_IRRIGATION_TIME}"));
            }

            if (WaterVolume > MAXIMUM_IRRIGATION_VOLUME)
            {
                errors.Add(new ValidationError(nameof(MaximumIrrigationTime),
                    $"{nameof(WaterVolume)} field cannot has value higher than {MAXIMUM_IRRIGATION_VOLUME}"));
            }

            return ConvertToArray(errors);
        }

        public class Factory : ICommandFactory
        {
            public ICommand Create(Hashtable obj)
            {
                return FromHashtable(obj);
            }
        }
    }
}