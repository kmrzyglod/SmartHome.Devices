using System.Net.NetworkInformation;

namespace EspIot.Infrastructure.Wifi
{
    public class NetworkConfig
    {
        public NetworkConfig(NetworkInterfaceType networkInterfaceType, string ssid, double rssi, string ip, string gatewayIp)
        {
            NetworkInterfaceType = networkInterfaceType;
            Ssid = ssid;
            Rssi = rssi;
            Ip = ip;
            GatewayIp = gatewayIp;
        }

        public NetworkInterfaceType NetworkInterfaceType { get; }
        public string Ssid { get; }
        public double Rssi { get; } //dB
        public string Ip { get; }
        public string GatewayIp { get; }
    }
}
