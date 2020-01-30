using EspIot.Core.Gpio;
using Windows.Devices.Gpio;

namespace EspIot.Drivers.LinearActuator
{
    //Simple driver witch uses H bridge builded with two relay switches to change motion direction
    //TODO implement internal actuator pulse counter handling
    public class LinearActuatorDriver
    {
        private readonly GpioController _gpioController;
        private readonly Mode _mode;
        private readonly GpioPin _pinExtension;
        private readonly GpioPin _pinReduction;

        public LinearActuatorDriver(GpioController gpioController, GpioPins pinExtension, GpioPins pinReduction, Mode mode)
        {
            _gpioController = gpioController;
            _mode = mode;
            _pinExtension = _gpioController.OpenPin((int)pinExtension);
            _pinExtension.SetDriveMode(GpioPinDriveMode.Output);
            _pinExtension.Write(_mode == Mode.DefaultHighState ? GpioPinValue.High : GpioPinValue.Low);

            _pinReduction = _gpioController.OpenPin((int)pinReduction);
            _pinReduction.SetDriveMode(GpioPinDriveMode.Output);
            _pinReduction.Write(_mode == Mode.DefaultHighState ? GpioPinValue.High : GpioPinValue.Low);
        }

        public void StartMovingExtensionDirection()
        {
            _pinExtension.Write(_mode == Mode.DefaultHighState ? GpioPinValue.Low : GpioPinValue.High);
            _pinReduction.Write(_mode == Mode.DefaultHighState ? GpioPinValue.High : GpioPinValue.Low);
        }

        public void StartMovingReductionDirection()
        {
            _pinExtension.Write(_mode == Mode.DefaultHighState ? GpioPinValue.High : GpioPinValue.Low);
            _pinReduction.Write(_mode == Mode.DefaultHighState ? GpioPinValue.Low : GpioPinValue.High);
        }

        public void StopMoving()
        {
            _pinExtension.Write(_mode == Mode.DefaultHighState ? GpioPinValue.High : GpioPinValue.Low);
            _pinReduction.Write(_mode == Mode.DefaultHighState ? GpioPinValue.High : GpioPinValue.Low);
        }
    }
}
