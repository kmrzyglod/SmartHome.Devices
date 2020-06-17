using System.Collections;
using EspIot.Application.Commands;
using EspIot.Application.Interfaces;
using EspIot.Core.Extensions;
using EspIot.Core.Messaging.Validation;
using GreenhouseController.Application.Services.Irrigation;

namespace GreenhouseController.Application.Commands.ScheduleIrrigation
{
    public class ScheduleIrrigationCommand : CommandBase
    {
        public const int MAXIMUM_IRRIGATION_TIME = 1200; 
        public const int MAXIMUM_IRRIGATION_VOLUME = 60; 

        //Irigation time in secons (optional)
        public int MaximumIrrigationTime { get; }

        //Water volume in liters (optional)
        public int WaterVolume { get; }
        public string IrrigationSchedule { get; }

        public ScheduleIrrigationCommand(string correlationId, int maximumIrrigationTime, int waterVolume,
            string irrigationSchedule) : base(correlationId)
        {
            MaximumIrrigationTime = maximumIrrigationTime;
            WaterVolume = waterVolume;
            IrrigationSchedule = irrigationSchedule;
        }

        public static ScheduleIrrigationCommand FromHashtable(Hashtable obj)
        {
            return new ScheduleIrrigationCommand(
                obj.GetString(nameof(CorrelationId)),
                obj.GetInt(nameof(MaximumIrrigationTime)), 
                obj.GetInt(nameof(WaterVolume)),
                obj.GetString(nameof(IrrigationSchedule)));
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