using System;
using System.Threading;
using Windows.Devices.Gpio;
using EspIot.Core.Gpio;
using EspIot.Drivers.ReedSwitch.Enums;
using EspIot.Drivers.ReedSwitch.Events;

namespace EspIot.Drivers.SparkFunRainGauge
{
    public class SparkFunRainGaugeDriver
    {
        private readonly GpioController _gpioController;
        private readonly GpioPin _pin;

        public SparkFunRainGaugeDriver(GpioController gpioController, GpioPins pin)
        {
            _gpioController = gpioController;
            _pin = _gpioController.OpenPin((int)pin);
            _pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            //while (true)
            //{
            //    var state = GetState();
            //    Console.WriteLine(state == 1 ? "High" : "Low");
            //    Thread.Sleep(500);
            //}
            //_pin.DebounceTimeout = TimeSpan.FromMilliseconds(1);
            _pin.ValueChanged += PinStateChangedHandler;
        }

        private  void PinStateChangedHandler(object sender, GpioPinValueChangedEventArgs e)
        {
            var state = GetState();
            Console.WriteLine(state == 1 ? "High" : "Low");
        }

        public int GetState()
        {
            return (int) _pin.Read();
        }
    }
}
