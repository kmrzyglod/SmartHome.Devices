using System;
using EspIot.Application.Events;

namespace GreenhouseController.Application.Events.Outbound
{
    public class IrrigationFinishedEvent: EventBase
    {
        public IrrigationFinishedEvent(float totalWaterVolume, float averageWaterFlow, float minWaterFlow, float maxWaterFlow, DateTime irrigationStartTime, DateTime irrigationEndTime)
        {
            TotalWaterVolume = totalWaterVolume;
            IrrigationStartTime = irrigationStartTime;
            IrrigationEndTime = irrigationEndTime;
            AverageWaterFlow = averageWaterFlow;
            MinWaterFlow = minWaterFlow;
            MaxWaterFlow = maxWaterFlow;
        }

        public float TotalWaterVolume { get; }
        public DateTime IrrigationStartTime { get; }
        public DateTime IrrigationEndTime { get; }
        public float AverageWaterFlow { get; }
        public float MinWaterFlow { get; }
        public float MaxWaterFlow { get; }
    }
}
