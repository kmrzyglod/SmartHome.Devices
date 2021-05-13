using System;
using System.Threading;
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
        private GpioPin _pin;
        private readonly GpioPins _gpioPin;
        private uint _precipitation; //unit:  [mm * 10000]
        private GpioPinValueChangedEventHandler _tickEventHandler => (_, eventArgs) =>
        {
            if (!_isMeasuring || eventArgs.Edge == GpioPinEdge.RisingEdge)
            {
                return;
            }

            Logger.Log("Rain gauge tick");

            _precipitation += ONE_PULSE_PRECIPITATION;
        };

        public SparkFunRainGaugeDriver(GpioController gpioController, GpioPins gpioPin)
        {
            _gpioController = gpioController;
            _gpioPin = gpioPin;
            Init();
        }

        private void Init()
        {
            _pin = _gpioController.OpenPin((int) _gpioPin);
            _pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _pin.DebounceTimeout = TimeSpan.FromMilliseconds(30);
            _pin.ValueChanged += _tickEventHandler;
        }

        public void StartMeasurement()
        {
            _isMeasuring = true;
            //Because of issue with triggering ValueChanged event we have to reinitialize pin continuously 
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(3_600_000);
                    _pin.ValueChanged -= _tickEventHandler;
                    _pin.Dispose();
                    if (!_isMeasuring)
                    {
                        break;
                    }
                   
                    Init();

                }
            }).Start();
        }

        public float GetPrecipitation()
        {
            return _precipitation / 10000f;
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