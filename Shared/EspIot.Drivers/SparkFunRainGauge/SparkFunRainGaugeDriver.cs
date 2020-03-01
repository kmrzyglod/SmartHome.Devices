using System;
using System.Collections;
using System.Threading;
using Windows.Devices.Gpio;
using EspIot.Core.Gpio;

namespace EspIot.Drivers.SparkFunRainGauge
{
    //https://cdn.sparkfun.com/assets/8/4/c/d/6/Weather_Sensor_Assembly_Updated.pdf
    public class SparkFunRainGaugeDriver
    {
        private const uint ONE_PULSE_PRECIPITATION = 2794;
        private readonly GpioController _gpioController;
        private GpioPin _pin;
        private bool _isMeasuring;
        private readonly GpioChangeCounter _pulseCounter;
        private Thread _measuringThread = new Thread(() => { });

        private uint _precipitation; //unit:  [mm * 10000]

        public SparkFunRainGaugeDriver(GpioController gpioController, GpioPins gpioPin)
        {
            _gpioController = gpioController;
            _pin = _gpioController.OpenPin((int) gpioPin);
            _pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _pulseCounter = new GpioChangeCounter(_pin) {Polarity = GpioChangePolarity.Rising};
        }

        public void StartMeasurement()
        {
            if (_measuringThread.IsAlive)
            {
                return; //don't start new measuring when previous is pending
            }

            _isMeasuring = true;

            _measuringThread = new Thread(() =>
            {
                _pulseCounter.Start();

                while (_isMeasuring)
                {
                    Thread.Sleep(10000);
                    var count = _pulseCounter.Read();
                    _pulseCounter.Reset();
                    Console.WriteLine("Pulse count: " + count.Count);
                    if (count.Count > 2)
                    {
                        continue; //workaround for some strange, random pin state changes which must be ommited 
                    }
                    _precipitation += ((uint)(count.Count / 2 * ONE_PULSE_PRECIPITATION));
                }

                _pulseCounter.Stop();
                _pulseCounter.Reset();
                Reset();
            });

            _measuringThread.Start();
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
            _measuringThread.Join();
            Reset();
        }
    }
}
