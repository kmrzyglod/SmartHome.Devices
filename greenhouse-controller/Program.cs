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
        public static void Main()
        {
            WifiDriver.ConnectToNetwork(ssid: "DOM_3", password: "network_home_n_zdc", authenticationType: Wireless80211Configuration.AuthenticationType.WPA2, encryptionType: Wireless80211Configuration.EncryptionType.WPA2_PSK);
            var reedSwitchDriver = new ReedSwitchDriver(GpioController.GetDefault(), Core.Gpio.GpioPins.GPIO_NUM_19);
            reedSwitchDriver.OnStateChanged += (sender, e) => Console.WriteLine("StateChanged");
            reedSwitchDriver.OnClosed += (sender, e) => Console.WriteLine("Closed");
            reedSwitchDriver.OnOpened += (sender, e) => Console.WriteLine("Opened");
            var bme28 = new Bme280Driver();
            bme28.Initialize();
            Console.WriteLine("Test");
            var iotHubClient = new AzureIotMqttClient("km-smart-home-iot-hub.azure-devices.net", 8883, "esp32-greenhouse", "SharedAccessSignature sr=km-smart-home-iot-hub.azure-devices.net%2Fdevices%2Fesp32-greenhouse&sig=LbwXOG6cycxQe%2FxsraTXKt85kL%2B5AX2j2Nojurs3XFI%3D&se=1591660364");
            iotHubClient.Connect();
            iotHubClient.Publish("\test\":\"test\"");
            while (true)
            {
                var t = bme28.ReadTemperature();
                Console.WriteLine(bme28.ReadTemperature().ToString() + "  " + bme28.ReadPreasure().ToString() + "  " + bme28.ReadHumidity().ToString());
                Thread.Sleep(1000);

            }
        }
    }
}
