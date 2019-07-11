using EspIot.Core.Messaging.Interfaces;

namespace Messages.Telemetry
{
    public class StatusMessage : IMessage
    {
        public string CorrelationId { get; }
        public double Temperature { get;}
        public double Pressure { get;}
        public double Humidity { get;}
        public int LightLevel { get;}
        public int SoilMoisture { get;}
        public int WaterFlow { get;}
        public bool IsDoorOpen { get;}
        public bool IsWindow1Open { get;}
        public bool IsWindow2Open { get;}
    }
}
