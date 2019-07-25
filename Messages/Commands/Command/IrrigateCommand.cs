using EspIot.Core.Messaging.Interfaces;

namespace Messages.Commands.Command
{
    class IrrigateCommand : ICommand
    {
        public IrrigateCommand(string correlationId, int irrigationTime, int waterVolume)
        {
            CorrelationId = correlationId;
            IrrigationTime = irrigationTime;
            WaterVolume = waterVolume;
        }

        public string CorrelationId { get;}
        //Irigation time in secons (optional)
        public int IrrigationTime { get;}
        //Water volume in liters (optional)
        public int WaterVolume { get; }

    }
}
