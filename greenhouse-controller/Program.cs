using EspIot.Core.Gpio;
using EspIot.Core.I2c;
using EspIot.Drivers.Bh1750;
using EspIot.Drivers.Bme280;
using EspIot.Drivers.DfrobotSoilMoistureSensor;
using EspIot.Drivers.LinearActuator;
using EspIot.Drivers.Mqtt;
using EspIot.Drivers.ReedSwitch;
using EspIot.Drivers.SeedstudioWaterFlowSensor;
using EspIot.Drivers.SoildStateRelay;
using EspIot.Drivers.StatusLed;
using EspIot.Drivers.Wifi;
using Json.NetMF;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Runtime.Native;
using System;
using System.Collections;
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
            public const string NETWORK_SSID = "DOM_3";
            public const string NETWORK_PASSWORD = "network_home_n_zdc";
            public const Wireless80211Configuration.AuthenticationType NETWORK_AUTH_TYPE = Wireless80211Configuration.AuthenticationType.WPA2;
            public const Wireless80211Configuration.EncryptionType NETWORK_ENCRYPTION_TYPE = Wireless80211Configuration.EncryptionType.WPA2_PSK;

            //MQTT broker config
            public const string BROKER_ADDRESS = "192.168.2.108";
            public const string DEVICE_ID = "esp32-greenhouse";
        }

        private class Test
        {
            public int TestInt { get; set; } = 12;
            public double TestDouble { get; set; } = 12.23;
            public string TestString { get; set; } = "Test";
            public DateTime TestDateTime { get; set; } = DateTime.MinValue;
        }

        public static void Main()
        {
            Configuration.SetPinFunction((int)GpioPins.GPIO_NUM_5, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction((int)GpioPins.GPIO_NUM_18, DeviceFunction.I2C1_CLOCK);

            Configuration.SetPinFunction((int)GpioPins.GPIO_NUM_19, DeviceFunction.I2C2_DATA);
            Configuration.SetPinFunction((int)GpioPins.GPIO_NUM_21, DeviceFunction.I2C2_CLOCK);

            var statusLedDriver = new StatusLed(GpioController.GetDefault(), GpioPins.GPIO_NUM_23, GpioPins.GPIO_NUM_22);

            WifiDriver.OnWifiConnected += () => { statusLedDriver.SetWifiConnected(); };
            WifiDriver.OnWifiDisconnected += () => { statusLedDriver.SetWifiDisconnected(); };

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
                });

            var reedSwitchDriver = new ReedSwitchDriver(GpioController.GetDefault(), GpioPins.GPIO_NUM_15);
            reedSwitchDriver.OnStateChanged += (sender, e) => Console.WriteLine("StateChanged");

            reedSwitchDriver.OnClosed += (sender, e) =>
            {
                Console.WriteLine("Closed");
                solidStateRelays.Off(0, 1, 2, 3);
            };

            reedSwitchDriver.OnOpened += (sender, e) =>
            {
                Console.WriteLine("Opened");
                solidStateRelays.On(0, 1, 2, 3);
            };

            //var waterFlowSensor = new WaterFlowSensor(GpioController.GetDefault(), GpioPins.GPIO_NUM_39, 1000);

            var iotHubClient = new MqttClientWrapper(DeviceConfiguration.BROKER_ADDRESS, DeviceConfiguration.DEVICE_ID);
            iotHubClient.OnMqttClientConnected += () => { statusLedDriver.SetMqttBrokerConnected(); };
            iotHubClient.OnMqttClientDisconnected += () => { statusLedDriver.SetMqttBrokerDisconnected(); };
            iotHubClient.Connect();
            iotHubClient.Subscribe(new[] { "devices/esp32-greenhouse/messages/devicebound/#" });
            //json test
            string json = JsonSerializer.SerializeObject(new Test());
            var deserialized = JsonSerializer.DeserializeString(json);
            if (deserialized is Hashtable)
            {
                Console.WriteLine(deserialized.ToString());   
            }

            var linearActuator = new LinearActuatorDriver(GpioController.GetDefault(), GpioPins.GPIO_NUM_33, GpioPins.GPIO_NUM_32, EspIot.Drivers.LinearActuator.Mode.DefaultHighState);
            linearActuator.StartMovingExtensionDirection();
            Thread.Sleep(5000);
            linearActuator.StartMovingReductionDirection();
            Thread.Sleep(5000);
            linearActuator.StopMoving();

            while (true)
            {
                // solidStateRelays.On(0, 1, 2, 3, 4, 5);
                Thread.Sleep(1000);
                //solidStateRelays.Off(0, 1, 2, 3, 4, 5);
                var t = bme28.ReadTemperature();
                Console.WriteLine(bme28.ReadTemperature().ToString() + "  " + bme28.ReadPreasure().ToString() + "  " + bme28.ReadHumidity().ToString());
                Console.WriteLine("Light sensor" + bh1750.GetLightLevelInLux() + " lux");
                Console.WriteLine("Soil moisture " + soilMoisture.GetUncalibratedMoisture() + " %");
                //Console.WriteLine("Water flow " + waterFlowSensor.GetMomentaryFlowValue() + " l/s");
                Console.WriteLine("");
                //   iotHubClient.Publish($"test {bme28.ReadTemperature().ToString() + "  " + bme28.ReadPreasure().ToString() + "  " + bme28.ReadHumidity().ToString()}");


                Thread.Sleep(1000);
            }
        }
    }
}
