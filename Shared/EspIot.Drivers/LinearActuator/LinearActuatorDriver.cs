using Windows.Devices.Gpio;
using EspIot.Core.Gpio;

namespace EspIot.Drivers.LinearActuator
{
    //Simple driver witch uses dual H bridge L298N https://www.sparkfun.com/datasheets/Robotics/L298_H_Bridge.pdf
    public class LinearActuatorDriver
    {
        private readonly GpioController _gpioController;
        private readonly GpioPin _pinEnable;
        private readonly GpioPin _pinExtension;
        private readonly GpioPin _pinReduction;

        public LinearActuatorDriver(GpioController gpioController, GpioPins pinExtension, GpioPins pinReduction,
            GpioPins pinEnable)
        {
            _gpioController = gpioController;

            _pinEnable = _gpioController.OpenPin((int) pinEnable);
            _pinEnable.SetDriveMode(GpioPinDriveMode.Output);
            _pinEnable.Write(GpioPinValue.Low);

            _pinExtension = _gpioController.OpenPin((int) pinExtension);
            _pinExtension.SetDriveMode(GpioPinDriveMode.Output);
            _pinExtension.Write(GpioPinValue.Low);

            _pinReduction = _gpioController.OpenPin((int) pinReduction);
            _pinReduction.SetDriveMode(GpioPinDriveMode.Output);
            _pinReduction.Write(GpioPinValue.Low);
        }

        public void StartMovingExtensionDirection()
        {
            _pinEnable.Write(GpioPinValue.High);
            _pinExtension.Write(GpioPinValue.High);
            _pinReduction.Write(GpioPinValue.Low);
        }

        public void StartMovingReductionDirection()
        {
            _pinEnable.Write(GpioPinValue.High);
            _pinExtension.Write(GpioPinValue.Low);
            _pinReduction.Write(GpioPinValue.High);
        }

        public void StopMoving()
        {
            _pinExtension.Write(GpioPinValue.Low);
            _pinReduction.Write(GpioPinValue.Low);
            _pinEnable.Write(GpioPinValue.Low);
        }
    }
}