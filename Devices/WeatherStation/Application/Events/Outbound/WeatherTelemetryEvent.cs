using System;
using EspIot.Application.Interfaces;
using EspIot.Drivers.SparkFunWindVane.Enums;

namespace WeatherStation.Application.Events.Outbound
{
    public class WeatherTelemetryEvent : IEvent
    {
        public WeatherTelemetryEvent(DateTime measurementStartTime,
            DateTime measurementEndTime,
            float temperature,
            float pressure,
            float humidity,
            int lightLevel,
            WindDirection currentWindDirection,
            WindDirection mostFrequentWindDirection,
            float averageWindSpeed,
            float maxWindSpeed,
            float minWindSpeed,
            float precipitation)
        {
            MeasurementStartTime = measurementStartTime;
            MeasurementEndTime = measurementEndTime;
            Temperature = temperature;
            Pressure = pressure;
            Humidity = humidity;
            LightLevel = lightLevel;
            CurrentWindDirection = currentWindDirection;
            MostFrequentWindDirection = mostFrequentWindDirection;
            AverageWindSpeed = averageWindSpeed;
            MaxWindSpeed = maxWindSpeed;
            MinWindSpeed = minWindSpeed;
            Precipitation = precipitation;
        }

        public DateTime MeasurementStartTime { get; }
        public DateTime MeasurementEndTime { get; }
        public double Temperature { get; } // [Celsius] 
        public double Pressure { get; } // [hPa]
        public double Humidity { get; } // [%]
        public int LightLevel { get; } // [lux]
        public WindDirection CurrentWindDirection { get; }
        public WindDirection MostFrequentWindDirection { get; }
        public float AverageWindSpeed { get; } // [m/s]
        public float MaxWindSpeed { get; } // [m/s]
        public float MinWindSpeed { get; } // [m/s]
        public float Precipitation { get; } // [mm]
    }
}