using greenhouse_controller.Drivers;
using greenhouse_controller.Drivers.Bme280;
using greenhouse_controller.Drivers.reed_switch;
using greenhouse_controller.Messaging.azure_iot_hub_mqtt_client;
using greenhouse_controller.Messaging.wifi;
using System;
using System.Net.NetworkInformation;
using System.Threading;
using Windows.Devices.Gpio;

namespace greenhouse_controller
{
    public class Program
    {
        public static class Configuration
        {
            //WIFI config
            public const string NETWORK_SSID = "";
            public const string NETWORK_PASSWORD = "";
            public const Wireless80211Configuration.AuthenticationType NETWORK_AUTH_TYPE = Wireless80211Configuration.AuthenticationType.WPA;
            public const Wireless80211Configuration.EncryptionType NETWORK_ENCRYPTION_TYPE = Wireless80211Configuration.EncryptionType.WPA_PSK;

            //MQTT broker config
            public const string BROKER_ADDRESS = "192.168.1.103";
            public const string DEVICE_ID = "esp32-greenhouse";
        }

        public static void Main()
        {
            WifiDriver.ConnectToNetwork(ssid: Configuration.NETWORK_SSID, password: Configuration.NETWORK_PASSWORD, authenticationType: Configuration.NETWORK_AUTH_TYPE, encryptionType: Configuration.NETWORK_ENCRYPTION_TYPE);
            var reedSwitchDriver = new ReedSwitchDriver(GpioController.GetDefault(), Core.Gpio.GpioPins.GPIO_NUM_19);
            reedSwitchDriver.OnStateChanged += (sender, e) => Console.WriteLine("StateChanged");
            reedSwitchDriver.OnClosed += (sender, e) => Console.WriteLine("Closed");
            reedSwitchDriver.OnOpened += (sender, e) => Console.WriteLine("Opened");
            var bme28 = new Bme280Driver();
            bme28.Initialize();
            var iotHubClient = new MqttClientWrapper("192.168.1.103", "esp32-greenhouse");
            iotHubClient.Connect();
            iotHubClient.Subscribe(new[] { "devices/esp32-greenhouse/messages/devicebound/#" });
            while (true)
            {
                var t = bme28.ReadTemperature();
                Console.WriteLine(bme28.ReadTemperature().ToString() + "  " + bme28.ReadPreasure().ToString() + "  " + bme28.ReadHumidity().ToString());
                iotHubClient.Publish($"test {bme28.ReadTemperature().ToString() + "  " + bme28.ReadPreasure().ToString() + "  " + bme28.ReadHumidity().ToString()}");
                Thread.Sleep(20000);
            }
        }
    }
}
