using Windows.Devices.Gpio;

namespace GreenhouseController.Drivers.ReedSwitch.Events
{
    public class ReedSwitchStateChangedEventArgs
    {
        public GpioPinValueChangedEventArgs GpioPinValueChangedEventArgs { get; set; }
    }
}
