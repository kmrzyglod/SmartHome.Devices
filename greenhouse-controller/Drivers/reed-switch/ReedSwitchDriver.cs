using greenhouse_controller.Core.Gpio;
using Windows.Devices.Gpio;
using greenhouse_controller.Drivers.Events;
using greenhouse_controller.Drivers.reed_switch.Enums;

namespace greenhouse_controller.Drivers.reed_switch
{
    public class ReedSwitchDriver
    {
        public event ReedSwitchOpenedEventHandler OnOpened;
        public event ReedSwitchClosedEventHandler OnClosed;
        public event ReedSwitchStateChangedEventHandler OnStateChanged;

        private readonly GpioPin _pin;
        private readonly GpioController _gpioController;

        public ReedSwitchDriver(GpioController gpioController, GpioPins pin)
        {
            _gpioController = gpioController;
            _pin = _gpioController.OpenPin((int)pin);
            _pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _pin.ValueChanged += PinValueChangedHandler;
        }

        private void PinValueChangedHandler(object sender, GpioPinValueChangedEventArgs e)
        {
            OnStateChanged(this, new ReedSwitchStateChangedEventArgs { GpioPinValueChangedEventArgs = e });

            if (e.Edge == GpioPinEdge.RisingEdge)
            {
                OnOpened(this, new ReedSwitchOpenedEventArgs());
            }
            else
            {
                OnClosed(this, new ReedSwitchClosedEventArgs());
            }
        }

        public ReedShiftState GetState()
        {
            return (ReedShiftState)(int) _pin.Read();
        }
    }
}
