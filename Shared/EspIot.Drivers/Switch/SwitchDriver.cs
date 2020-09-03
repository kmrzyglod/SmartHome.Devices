using System;
using System.Threading;
using Windows.Devices.Gpio;
using EspIot.Core.Gpio;
using EspIot.Drivers.Switch.Enums;
using EspIot.Drivers.Switch.Events;

namespace EspIot.Drivers.Switch
{
    public class SwitchDriver: IDisposable
    {
        public event SwitchOpenedEventHandler OnOpened;
        public event SwitchClosedEventHandler OnClosed;
        public event SwitchStateChangedEventHandler OnStateChanged;

        private GpioPin _pin;
        private readonly GpioController _gpioController;
        private readonly GpioPins _gpioPin;
        private readonly GpioPinDriveMode _pinMode;
        private readonly TimeSpan _debounceTimeout;
        private bool _isDisposed = false;
        private Thread _reinitializeThread;

        public SwitchDriver(GpioController gpioController, GpioPins pin, GpioPinDriveMode pinMode, TimeSpan debounceTimeout = default)
        {
            if (debounceTimeout == default)
            {
                debounceTimeout = TimeSpan.FromMilliseconds(20);
            }
            _gpioController = gpioController;
            _gpioPin = pin;
            _pinMode = pinMode;
            _debounceTimeout = debounceTimeout;
            Init();

            _reinitializeThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(30000);
                    _pin.ValueChanged -= PinValueChangedHandler;
                    _pin.Dispose();
                    if (_isDisposed)
                    {
                        break;
                    }

                    Init();
                }
            });
            _reinitializeThread.Start();

        }

        private void Init()
        {
            _pin = _gpioController.OpenPin((int)_gpioPin);
            _pin.SetDriveMode(_pinMode);
            _pin.DebounceTimeout = _debounceTimeout;
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

        public void Dispose()
        {
            _isDisposed = true;
            _reinitializeThread.Join();
        }
    }
}
