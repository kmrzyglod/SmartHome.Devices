﻿using System;
using EspIot.Drivers.SparkFunWindVane.Enums;
using EspIot.Application.Events;

namespace WeatherStation.Application.Events.Outbound
{
    public class WeatherTelemetryEvent : EventBase
    {
        public WeatherTelemetryEvent(DateTime measurementStartTime,
            DateTime measurementEndTime,
            double temperature,
            double pressure,
            double humidity,
            int lightLevel,
            WindDirection currentWindDirection,
            WindDirection mostFrequentWindDirection,
            double averageWindSpeed,
            double maxWindSpeed,
            double minWindSpeed,
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
        public double AverageWindSpeed { get; } // [m/s]
        public double MaxWindSpeed { get; } // [m/s]
        public double MinWindSpeed { get; } // [m/s]
        public float Precipitation { get; } // [mm]
    }
}