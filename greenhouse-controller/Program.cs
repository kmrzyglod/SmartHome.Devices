using greenhouse_controller.Drivers.reed_switch;
using System;
using System.Threading;
using Windows.Devices.Gpio;

namespace greenhouse_controller
{
    public class Program
    {
        public static void Main()
        {
            var reedSwitchDriver = new ReedSwitchDriver(GpioController.GetDefault(), Core.Gpio.GpioPins.GPIO_NUM_19);
            reedSwitchDriver.OnStateChanged += (sender, e) => Console.WriteLine("StateChanged");
            reedSwitchDriver.OnClosed += (sender, e) => Console.WriteLine("Closed");
            reedSwitchDriver.OnOpened += (sender, e) => Console.WriteLine("Opened");
            Console.WriteLine("Test");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
