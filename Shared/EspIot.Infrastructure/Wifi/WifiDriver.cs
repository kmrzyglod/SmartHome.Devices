using System;
using System.Net.NetworkInformation;
using System.Threading;
using Windows.Devices.WiFi;
using EspIot.Core.Helpers;
using EspIot.Infrastructure.Wifi.Events;

namespace EspIot.Infrastructure.Wifi
{
    public static class WifiDriver
    {
        private static Thread _wifiStatusWatcher;
        private static bool _connectionStatus;
        private static readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        public static event WifiConnectedEventHandler OnWifiConnected;
        public static event WifiDuringConnectionEventHandler OnWifiDuringConnection;
        public static event WifiDisconnectedEventHandler OnWifiDisconnected;
        private static WiFiAdapter _wifiAdapter;

        public static NetworkConfig GetNetworkConfig()
        {
            var nis = NetworkInterface.GetAllNetworkInterfaces();

            if (nis.Length <= 0)
            {
                return new NetworkConfig(NetworkInterfaceType.Unknown, string.Empty, double.NaN, string.Empty,
                    string.Empty);
            }

            // get the first interface
            var ni = nis[0];
            if (ni.NetworkInterfaceType != NetworkInterfaceType.Wireless80211)
            {
                return new NetworkConfig(ni.NetworkInterfaceType, string.Empty, double.NaN, ni.IPv4Address,
                    ni.IPv4GatewayAddress);
            }

            var wc = Wireless80211Configuration.GetAllWireless80211Configurations()[ni.SpecificConfigId];
            _wifiAdapter.ScanAsync();
            _autoResetEvent.WaitOne();
            double rssi = double.NaN;
            foreach (var networkReportAvailableNetwork in _wifiAdapter.NetworkReport.AvailableNetworks)
            {
                if (networkReportAvailableNetwork.Ssid == wc.Ssid)
                {
                    rssi = networkReportAvailableNetwork.NetworkRssiInDecibelMilliwatts;
                }
            }

            return new NetworkConfig(ni.NetworkInterfaceType, wc.Ssid, rssi, ni.IPv4Address, ni.IPv4GatewayAddress);
        }

        private static void InitWifiAdpater()
        {
            if (_wifiAdapter != null)
            {
                return;
            }

            _wifiAdapter = WiFiAdapter.FindAllAdapters()[0];
            _wifiAdapter.AvailableNetworksChanged += (_, __) => _autoResetEvent.Set();
        }

        public static void ConnectToNetwork()
        {
            if (_connectionStatus)
            {
                return;
            }

            var nis = NetworkInterface.GetAllNetworkInterfaces();

            if (nis.Length > 0)
            {
                // get the first interface
                var ni = nis[0];

                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    // network interface is Wi-Fi
                    Logger.Log("Network connection is: Wi-Fi");
                    var wc = Wireless80211Configuration.GetAllWireless80211Configurations()[ni.SpecificConfigId];
                    wc.Options = Wireless80211Configuration.ConfigurationOptions.AutoConnect;
                        wc.SaveConfiguration();
                    OnWifiDuringConnection();
                }
                else
                {
                    throw new NotSupportedException("ERROR: there is no wifi network interface configured.");
                }

                // wait for DHCP to complete
                WaitIp();
                OnWifiConnected();
                _connectionStatus = true;
                InitWifiAdpater();
                StartWifiStatusWatcher();
            }
            else
            {
                throw new NotSupportedException("ERROR: there is no wifi network interface configured.");
            }
        }

        private static void WaitIp()
        {
            Logger.Log("Waiting for IP...");

            while (true)
            {
                var ni = NetworkInterface.GetAllNetworkInterfaces()[0];
                if (ni.IPv4Address != null && ni.IPv4Address.Length > 0)
                {
                    if (ni.IPv4Address[0] != '0')
                    {
                        Logger.Log($"We have an IP: {ni.IPv4Address}");
                        break;
                    }
                }

                Thread.Sleep(300);
            }

            SetDateTime();
        }

        private static void SetDateTime()
        {
            Logger.Log("Setting up system clock...");

            // if SNTP is available and enabled on target device this can be skipped because we should have a valid date & time
            while (DateTime.UtcNow.Year < 2018)
            {
                Logger.Log("Waiting for valid date time...");
                // wait for valid date & time
                Thread.Sleep(1000);
            }

            Logger.Log($"System time is: {DateTime.UtcNow.ToString()}");
        }

        private static void StartWifiStatusWatcher()
        {
            if (_wifiStatusWatcher != null)
            {
                return;
            }

            _wifiStatusWatcher = new Thread(() =>
            {
                while (true)
                {
                    var ni = NetworkInterface.GetAllNetworkInterfaces()[0];
                    if (ni.IPv4Address != null && ni.IPv4Address.Length > 0 && ni.IPv4Address[0] != '0')
                    {
                        if (!_connectionStatus)
                        {
                            OnWifiConnected();
                            _connectionStatus = true;
                        }
                    }
                    else if (_connectionStatus)
                    {
                        _connectionStatus = false;
                        OnWifiDuringConnection();
                    }

                    Thread.Sleep(1000);
                }
            });
            _wifiStatusWatcher.Start();
        }
    }
}