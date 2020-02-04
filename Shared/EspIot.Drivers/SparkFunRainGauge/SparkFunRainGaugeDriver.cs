using System;
using System.Threading;
using Windows.Devices.Adc;
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
            //var adc1 = AdcController.GetDefault();
            //var adcChannel = adc1.OpenChannel(0);

            //while (true)
            //{
            //    Thread.Sleep(100);
            //    Console.WriteLine(adcChannel.ReadValue().ToString());
            //}
            _gpioController = gpioController;
            _pin = _gpioController.OpenPin((int)pin);
            //_pin.Write(GpioPinValue.Low);
            _pin.SetDriveMode(GpioPinDriveMode.Input);
            //while (true)
            //{
            //    var state = GetState();
            //    Console.WriteLine(state == 1 ? "High" : "Low");
            //    Thread.Sleep(500);
            //}
            _pin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            _pin.ValueChanged += PinStateChangedHandler;
            // Create a Counter passing in the GPIO pin
            //GpioChangeCounter gpcc = new GpioChangeCounter(_pin);

            //while (true)
            //{
            //    // Counter both raising and falling edges
            //    gpcc.Polarity = GpioChangePolarity.Rising;

            //    Console.WriteLine($"Counter pin created");

            //    // Start counter
            //    gpcc.Start();

            //    // Read count before we start PWM ( should be 0 )
            //    // We want to save the start relative time 
            //    GpioChangeCount count1 = gpcc.Read();

            //    // Wait 1 Sec
            //    Thread.Sleep(10000);

            //    // Read current count 
            //    GpioChangeCount count2 = gpcc.Read();

            //    // Stop PWM signal & counter
           
            //    gpcc.Stop();
            //    Console.WriteLine($"Counter start {count1} stop {count2}");
            //    gpcc.Reset();
            //} 


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
