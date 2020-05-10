using Windows.Devices.Gpio;

namespace EspIot.Drivers.Switch.Events
{
    public class SwitchStateChangedEventArgs
    {
        public GpioPinValueChangedEventArgs GpioPinValueChangedEventArgs { get; set; }
    }
}
