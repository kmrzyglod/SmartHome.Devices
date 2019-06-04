using Windows.Devices.Gpio;

namespace greenhouse_controller.Drivers.Events
{
    public class ReedSwitchStateChangedEventArgs
    {
        public GpioPinValueChangedEventArgs GpioPinValueChangedEventArgs { get; set; }
    }
}
