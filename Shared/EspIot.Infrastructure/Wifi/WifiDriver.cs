using System;
using System.Net.NetworkInformation;
using System.Threading;
using EspIot.Infrastructure.Wifi.Events;

namespace EspIot.Infrastructure.Wifi
{
    public static class WifiDriver
    {
        public static event WifiConnectedEventHandler OnWifiConnected;
        public static event WifiDisconnectedEventHandler OnWifiDisconnected;
        private static Thread _wifiStatusWatcher;
        private static bool _connectionStatus = false;

        public static void ConnectToNetwork()
        {
            NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();

            if (nis.Length > 0)
            {
                // get the first interface
                var ni = nis[0];

                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    // network interface is Wi-Fi
                    Console.WriteLine("Network connection is: Wi-Fi");
                }
                else
                {
                    throw new NotSupportedException("ERROR: there is no wifi network interface configured.");
                }

                // wait for DHCP to complete
                WaitIp();
                OnWifiConnected();
                _connectionStatus = true;
                StartWifiStatusWatcher();
            }
            else
            {
                throw new NotSupportedException("ERROR: there is no wifi network interface configured.");
            }
        }

        private static void WaitIp()
        {
            Console.WriteLine("Waiting for IP...");

            while (true)
            {
                var ni = NetworkInterface.GetAllNetworkInterfaces()[0];
                if (ni.IPv4Address != null && ni.IPv4Address.Length > 0)
                {
                    if (ni.IPv4Address[0] != '0')
                    {
                        Console.WriteLine($"We have an IP: {ni.IPv4Address}");
                        break;
                    }
                }

                Thread.Sleep(300);
            }

            SetDateTime();
        }

        private static void SetDateTime()
        {
            Console.WriteLine("Setting up system clock...");

            // if SNTP is available and enabled on target device this can be skipped because we should have a valid date & time
            while (DateTime.UtcNow.Year < 2018)
            {
                Console.WriteLine("Waiting for valid date time...");
                // wait for valid date & time
                Thread.Sleep(1000);
            }

            Console.WriteLine($"System time is: {DateTime.UtcNow.ToString()}");
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
                        OnWifiDisconnected();
                    }

                    Thread.Sleep(1000);
                }
            });
            _wifiStatusWatcher.Start();
        }
    }
}
