using System;
using Windows.Devices.Gpio;
using EspIot.Core.Gpio;
using EspIot.Drivers.Switch.Enums;
using EspIot.Drivers.Switch.Events;

namespace EspIot.Drivers.Switch
{
    public class SwitchDriver
    {
        public event SwitchOpenedEventHandler OnOpened;
        public event SwitchClosedEventHandler OnClosed;
        public event SwitchStateChangedEventHandler OnStateChanged;

        private readonly GpioPin _pin;
        private readonly GpioController _gpioController;

        public SwitchDriver(GpioController gpioController, GpioPins pin, GpioPinDriveMode pinMode, TimeSpan debounceTimeout = default)
        {
            if (debounceTimeout == default)
            {
                debounceTimeout = TimeSpan.FromMilliseconds(20);
            }
            _gpioController = gpioController;
            _pin = _gpioController.OpenPin((int)pin);
            _pin.SetDriveMode(pinMode);
            _pin.DebounceTimeout = debounceTimeout;
            _pin.ValueChanged += PinValueChangedHandler;
        }

        private void PinValueChangedHandler(object sender, GpioPinValueChangedEventArgs e)
        {
            OnStateChanged?.Invoke(this, new SwitchStateChangedEventArgs { GpioPinValueChangedEventArgs = e });

            var currentState = GetState();
            if (currentState == SwitchState.Opened)
            {
                OnOpened?.Invoke(this, new SwitchOpenedEventArgs());
            }
            else
            {
                OnClosed?.Invoke(this, new SwitchClosedEventArgs());
            }
        }

        public SwitchState GetState()
        {
            return (SwitchState)(int) _pin.Read();
        }
    }
}
