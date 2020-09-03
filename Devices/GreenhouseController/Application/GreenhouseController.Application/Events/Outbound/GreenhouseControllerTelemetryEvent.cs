using System;
using EspIot.Application.Interfaces;

namespace GreenhouseController.Application.Events.Outbound
{
    public class GreenhouseControllerTelemetryEvent : IEvent
    {
        public GreenhouseControllerTelemetryEvent(DateTime measurementStartTime,
            DateTime measurementEndTime, float temperature, float pressure, float humidity, int lightLevel,
            int soilMoisture, bool isDoorOpen)
        {
            MeasurementStartTime = measurementStartTime;
            MeasurementEndTime = measurementEndTime;
            Temperature = temperature;
            Pressure = pressure;
            Humidity = humidity;
            LightLevel = lightLevel;
            SoilMoisture = soilMoisture;
            IsDoorOpen = isDoorOpen;
        }

        public DateTime MeasurementStartTime { get; }
        public DateTime MeasurementEndTime { get; }
        public float Temperature { get; }
        public float Pressure { get; }
        public float Humidity { get; }
        public int LightLevel { get; }
        public int SoilMoisture { get; }
        public bool IsDoorOpen { get; }
    }
}