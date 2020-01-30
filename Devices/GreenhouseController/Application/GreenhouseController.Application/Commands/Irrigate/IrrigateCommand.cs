using System.Collections;
using EspIot.Core.Extensions;
using EspIot.Core.Messaging.Concrete;
using GreenhouseController.Application.Services.Irrigation;

namespace GreenhouseController.Application.Commands.Irrigate
{
    internal class IrrigateCommand : CommandBase
    {
        //Irigation time in secons (optional)
        public int IrrigationTime { get; }

        //Water volume in liters (optional)
        public int WaterVolume { get; }

        public IrrigateCommand(string correlationId, int irrigationTime, int waterVolume) : base(correlationId)
        {
            IrrigationTime = irrigationTime;
            WaterVolume = waterVolume;
        }

        public static IrrigateCommand FromHashtable(Hashtable obj)
        {
            return new IrrigateCommand(
                obj.GetString(nameof(CorrelationId)),
                obj.GetInt(nameof(IrrigationTime)),
                obj.GetInt(nameof(WaterVolume)));
        }

        public override string PartitionKey { get; } = nameof(IrrigationService);
    }
}