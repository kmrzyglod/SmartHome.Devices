using System;
using System.Threading;
using Windows.Devices.Gpio;
using EspIot.Core.Gpio;
using EspIot.Core.Helpers;
using nanoFramework.Runtime.Native;

namespace EspIot.Drivers.SparkFunRainGauge
{
    //https://cdn.sparkfun.com/assets/8/4/c/d/6/Weather_Sensor_Assembly_Updated.pdf
    public class SparkFunRainGaugeDriver
    {
        private const uint ONE_PULSE_PRECIPITATION = 2794;
        private readonly GpioController _gpioController;
        private readonly GpioPins _gpioPin;
        private bool _isMeasuring;
        private GpioPin _pin;
        private Thread _pinResettingThread;
        private uint _precipitation; //unit:  [mm * 10000]

        public SparkFunRainGaugeDriver(GpioController gpioController, GpioPins gpioGpioPin)
        {
            _gpioPin = gpioGpioPin;
            _gpioController = gpioController;
            InitPin();
            ReinitializePinPeriodically();
        }


        //Workaround - because of some bug in nanoframework ValueChanged stops triggering after some period of time 
        private void ReinitializePinPeriodically()
        {
            _pinResettingThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(30000);
                    _pin.Dispose();
                    InitPin();
                }
            });
            _pinResettingThread.Start();
        }

        private void InitPin()
        {
            _pin = _gpioController.OpenPin((int) _gpioPin);
            _pin.SetDriveMode(GpioPinDriveMode.Input);
            _pin.DebounceTimeout = TimeSpan.FromMilliseconds(30);
            _pin.ValueChanged += (_, eventArgs) =>
            {
                if (!_isMeasuring || eventArgs.Edge == GpioPinEdge.RisingEdge)
                {
                    return;
                }

                Logger.Log("Pulse count");

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