using System;
using EspIot.Application.Interfaces;

namespace GreenhouseController.Application.Events.Outbound
{
    public class TelemetryEvent : IEvent
    {
        public TelemetryEvent(DateTime measurementStartTime,
            DateTime measurementEndTime, float temperature, float pressure, float humidity, int lightLevel,
            int soilMoisture, float averageWaterFlow, float minWaterFlow, float maxWaterFlow, float totalWaterFlow,
            bool isDoorOpen)
        {
            MeasurementStartTime = measurementStartTime;
            MeasurementEndTime = measurementEndTime;
            Temperature = temperature;
            Pressure = pressure;
            Humidity = humidity;
            LightLevel = lightLevel;
            SoilMoisture = soilMoisture;
            AverageWaterFlow = averageWaterFlow;
            IsDoorOpen = isDoorOpen;
            MinWaterFlow = minWaterFlow;
            MaxWaterFlow = maxWaterFlow;
            TotalWaterFlow = totalWaterFlow;
        }

        public DateTime MeasurementStartTime { get; }
        public DateTime MeasurementEndTime { get; }
        public float Temperature { get; }
        public float Pressure { get; }
        public float Humidity { get; }
        public int LightLevel { get; }
        public int SoilMoisture { get; }
        public float AverageWaterFlow { get; }
        public float MinWaterFlow { get; }
        public float MaxWaterFlow { get; }
        public float TotalWaterFlow { get; }
        public bool IsDoorOpen { get; }
    }
}