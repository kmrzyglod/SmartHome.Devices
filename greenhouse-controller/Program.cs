using GreenhouseController.Core.Gpio;
using GreenhouseController.Core.I2c;
using GreenhouseController.Drivers.Bh1750;
using GreenhouseController.Drivers.Bme280;
using GreenhouseController.Drivers.DfrobotSoilMoistureSensor;
using GreenhouseController.Drivers.ReedSwitch;
using GreenhouseController.Drivers.SeedstudioWaterFlowSensor;
using GreenhouseController.Drivers.SoildStateRelay;
using GreenhouseController.Drivers.Wifi;
using nanoFramework.Hardware.Esp32;
using System;
using System.Net.NetworkInformation;
using System.Threading;
using Windows.Devices.Adc;
using Windows.Devices.Gpio;

namespace GreenhouseController
{
    public class Program
    {
        public static class DeviceConfiguration
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
            Configuration.SetPinFunction((int)GpioPins.GPIO_NUM_5, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction((int)GpioPins.GPIO_NUM_18, DeviceFunction.I2C1_CLOCK);

            Configuration.SetPinFunction((int)GpioPins.GPIO_NUM_19, DeviceFunction.I2C2_DATA);
            Configuration.SetPinFunction((int)GpioPins.GPIO_NUM_21, DeviceFunction.I2C2_CLOCK);

            WifiDriver.ConnectToNetwork(ssid: DeviceConfiguration.NETWORK_SSID, password: DeviceConfiguration.NETWORK_PASSWORD, authenticationType: DeviceConfiguration.NETWORK_AUTH_TYPE, encryptionType: DeviceConfiguration.NETWORK_ENCRYPTION_TYPE);


            var bme28 = new Bme280(I2cControllerName.I2C1);
            bme28.Initialize();

            var bh1750 = new Bh1750(I2cControllerName.I2C2);

            var soilMoisture = new DfrobotSoilMoistureSensor(0);

            var solidStateRelays = new SolidStateRelays(GpioController.GetDefault(), new[]{
                (short) GpioPins.GPIO_NUM_14,
                (short) GpioPins.GPIO_NUM_27,
                (short) GpioPins.GPIO_NUM_26,
                (short) GpioPins.GPIO_NUM_25,
                (short) GpioPins.GPIO_NUM_33,
                (short) GpioPins.GPIO_NUM_32
                });

            var reedSwitchDriver = new ReedSwitchDriver(GpioController.GetDefault(), Core.Gpio.GpioPins.GPIO_NUM_15);
            reedSwitchDriver.OnStateChanged += (sender, e) => Console.WriteLine("StateChanged");
            
            reedSwitchDriver.OnClosed += (sender, e) =>
            {
                Console.WriteLine("Closed");
                solidStateRelays.Off(0, 1, 2, 3, 4, 5);
            };
            
            reedSwitchDriver.OnOpened += (sender, e) =>
            {
                Console.WriteLine("Opened");
                solidStateRelays.On(0, 1, 2, 3, 4, 5);
            };

            var waterFlowSensor = new WaterFlowSensor(GpioController.GetDefault(), GpioPins.GPIO_NUM_39, 1000);


            //var iotHubClient = new MqttClientWrapper("192.168.1.103", "esp32-greenhouse");
            //iotHubClient.Connect();
            //iotHubClient.Subscribe(new[] { "devices/esp32-greenhouse/messages/devicebound/#" });
            while (true)
            {
                // solidStateRelays.On(0, 1, 2, 3, 4, 5);
                Thread.Sleep(1000);
                //solidStateRelays.Off(0, 1, 2, 3, 4, 5);
                var t = bme28.ReadTemperature();
                Console.WriteLine(bme28.ReadTemperature().ToString() + "  " + bme28.ReadPreasure().ToString() + "  " + bme28.ReadHumidity().ToString());
                Console.WriteLine("Light sensor" + bh1750.GetLightLevelInLux() + " lux");
                Console.WriteLine("Soil moisture " + soilMoisture.GetUncalibratedMoisture() + " %");
                Console.WriteLine("Water flow " + waterFlowSensor.GetMomentaryFlowValue() + " l/s");
                Console.WriteLine("");
                //   iotHubClient.Publish($"test {bme28.ReadTemperature().ToString() + "  " + bme28.ReadPreasure().ToString() + "  " + bme28.ReadHumidity().ToString()}");
                Thread.Sleep(1000);
            }
        }
    }
}
