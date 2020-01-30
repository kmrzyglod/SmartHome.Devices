using Windows.Devices.Gpio;

namespace EspIot.Drivers.ReedSwitch.Events
{
    public class ReedSwitchStateChangedEventArgs
    {
        public GpioPinValueChangedEventArgs GpioPinValueChangedEventArgs { get; set; }
    }
}
