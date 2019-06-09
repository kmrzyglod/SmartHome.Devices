using greenhouse_controller.Drivers;
using greenhouse_controller.Drivers.BME280;
using greenhouse_controller.Drivers.reed_switch;
using System;
using System.Threading;
//using uPLibrary.Networking.M2Mqtt;
using Windows.Devices.Gpio;

namespace greenhouse_controller
{
    public class Program
    {
        public static void Main()
        {
            //MqttClient client = null;
            var reedSwitchDriver = new ReedSwitchDriver(GpioController.GetDefault(), Core.Gpio.GpioPins.GPIO_NUM_19);
            reedSwitchDriver.OnStateChanged += (sender, e) => Console.WriteLine("StateChanged");
            reedSwitchDriver.OnClosed += (sender, e) => Console.WriteLine("Closed");
            reedSwitchDriver.OnOpened += (sender, e) => Console.WriteLine("Opened");
            var bme28 = new BME280();
            bme28.Initialize();
            //var bme280 = new BME280();
            //bme280.Initialise();
            //var t = bme280.GetTemperature();
            //Console.WriteLine(t.ToString());
            // bme280.Initialize();
            // bme280.PrepareToRead();
            // bme280.Read();
            // Console.WriteLine("BME 280 " + bme280.PressureInPa + " " + bme280.TemperatureInCelcius);
            Console.WriteLine("Test");
            while (true)
            {
                var t = bme28.ReadTemperature();
                Console.WriteLine(bme28.ReadTemperature().ToString() + "  " + bme28.ReadPreasure().ToString() + "  " + bme28.ReadHumidity().ToString());
                Thread.Sleep(1000);

            }
        }
    }
}
