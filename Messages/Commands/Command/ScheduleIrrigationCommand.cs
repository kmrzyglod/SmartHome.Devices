using EspIot.Core.Messaging.Interfaces;

namespace Messages.Commands.Command
{
    public class ScheduleIrrigationCommand : ICommand
    {
        public ScheduleIrrigationCommand(string correlationId, int irrigationTime, int waterVolume, string irrigationSchedule)
        {
            CorrelationId = correlationId;
            IrrigationTime = irrigationTime;
            WaterVolume = waterVolume;
            IrrigationSchedule = irrigationSchedule;
        }

        public string CorrelationId { get; }
        //Irigation time in secons (optional)
        public int IrrigationTime { get; }
        //Water volume in liters (optional)
        public int WaterVolume { get; }
        public string IrrigationSchedule { get;} 
    }
}
