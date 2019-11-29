using System.Collections;
using EspIot.Core.Extensions;
using EspIot.Core.Messaging.Concrete;
using Infrastructure.Services.Irrigation;

namespace Infrastructure.Commands.Command
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