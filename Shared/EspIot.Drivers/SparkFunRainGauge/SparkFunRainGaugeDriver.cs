using System;
using Windows.Devices.Gpio;
using EspIot.Core.Gpio;
using EspIot.Core.Helpers;

namespace EspIot.Drivers.SparkFunRainGauge
{
    //https://cdn.sparkfun.com/assets/8/4/c/d/6/Weather_Sensor_Assembly_Updated.pdf
    public class SparkFunRainGaugeDriver
    {
        private const uint ONE_PULSE_PRECIPITATION = 2794;
        private readonly GpioController _gpioController;
        private bool _isMeasuring;
        private readonly GpioPin _pin;
        private uint _precipitation; //unit:  [mm * 10000]

        public SparkFunRainGaugeDriver(GpioController gpioController, GpioPins gpioPin)
        {
            _gpioController = gpioController;
            _pin = _gpioController.OpenPin((int) gpioPin);
            _pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _pin.DebounceTimeout = TimeSpan.FromMilliseconds(30);
            _pin.ValueChanged += (_, eventArgs) =>
            {
                if (!_isMeasuring || eventArgs.Edge == GpioPinEdge.RisingEdge)
                {
                    return;
                }

                Logger.Log("Rain gauge tick");

                _precipitation += ONE_PULSE_PRECIPITATION;
            };
        }

        public void StartMeasurement()
        {
            _isMeasuring = true;
        }

        public uint GetPrecipitation()
        {
            return _precipitation;
        }

        public void Reset()
        {
            _precipitation = 0;
        }

        public void StopMeasurement()
        {
            _isMeasuring = false;
            Reset();
        }
    }
}