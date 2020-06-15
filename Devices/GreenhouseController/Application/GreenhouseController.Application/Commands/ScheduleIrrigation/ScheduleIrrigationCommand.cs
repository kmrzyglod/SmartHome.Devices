using System.Collections;
using EspIot.Application.Commands;
using EspIot.Core.Extensions;
using GreenhouseController.Application.Services.Irrigation;

namespace GreenhouseController.Application.Commands.ScheduleIrrigation
{
    public class ScheduleIrrigationCommand : CommandBase
    {
        //Irigation time in secons (optional)
        public int IrrigationTime { get; }

        //Water volume in liters (optional)
        public int WaterVolume { get; }
        public string IrrigationSchedule { get; }

        public ScheduleIrrigationCommand(string correlationId, int irrigationTime, int waterVolume,
            string irrigationSchedule) : base(correlationId)
        {
            IrrigationTime = irrigationTime;
            WaterVolume = waterVolume;
            IrrigationSchedule = irrigationSchedule;
        }

        public static ScheduleIrrigationCommand FromHashtable(Hashtable obj)
        {
            return new ScheduleIrrigationCommand(
                obj.GetString(nameof(CorrelationId)),
                obj.GetInt(nameof(IrrigationTime)), 
                obj.GetInt(nameof(WaterVolume)),
                obj.GetString(nameof(IrrigationSchedule)));
        }

        public override string PartitionKey { get; } = nameof(IrrigationService);
    }
}