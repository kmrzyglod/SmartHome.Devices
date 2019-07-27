﻿using EspIot.Core.Messaging.Interfaces;

namespace Messages.Telemetry
{
    public class StatusMessage : IMessage
    {
        public StatusMessage(string correlationId, double temperature, double pressure, double humidity, int lightLevel, int soilMoisture, int waterFlow, bool isDoorOpen, bool isWindow1Open, bool isWindow2Open)
        {
            CorrelationId = correlationId;
            Temperature = temperature;
            Pressure = pressure;
            Humidity = humidity;
            LightLevel = lightLevel;
            SoilMoisture = soilMoisture;
            WaterFlow = waterFlow;
            IsDoorOpen = isDoorOpen;
            IsWindow1Open = isWindow1Open;
            IsWindow2Open = isWindow2Open;
        }

        public string CorrelationId { get; }
        public double Temperature { get; }
        public double Pressure { get; }
        public double Humidity { get; }
        public int LightLevel { get; }
        public int SoilMoisture { get; }
        public int WaterFlow { get; }
        public bool IsDoorOpen { get; }
        public bool IsWindow1Open { get; }
        public bool IsWindow2Open { get; }
    }
}