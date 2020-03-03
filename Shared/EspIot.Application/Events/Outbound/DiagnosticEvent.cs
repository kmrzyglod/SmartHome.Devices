using EspIot.Core.Messaging.Interfaces;

namespace EspIot.Application.Events.Outbound
{
    public class DiagnosticEvent : IEvent
    {
        public DiagnosticEvent(string ssid, double rssi, string ip, string gatewayIp, uint freeHeapMemory)
        {
            Ssid = ssid;
            Rssi = rssi;
            Ip = ip;
            GatewayIp = gatewayIp;
            FreeHeapMemory = freeHeapMemory;
        }

        public string Ssid { get; }
        public double Rssi { get; } //dB miliwats
        public string Ip { get; }
        public string GatewayIp { get; }
        public uint FreeHeapMemory { get; } //in bytes
    }
}