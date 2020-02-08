using System;
using System.Collections;
using System.Threading;
using Windows.Devices.Gpio;
using EspIot.Core.Extensions;
using EspIot.Core.Gpio;
using EspIot.Drivers.SparkFunRainGauge.Enums;

namespace EspIot.Drivers.SparkFunRainGauge
{
    public class SparkFunRainGaugeDriver
    {
        //https://cdn.sparkfun.com/assets/8/4/c/d/6/Weather_Sensor_Assembly_Updated.pdf
        private const uint ONE_PULSE_PRECIPITATION = 2794;
        private readonly GpioController _gpioController;
        private readonly GpioPin _pin;
        private readonly GpioChangeCounter _pulseCounter;
        private bool _isMeasuring;

        private readonly RainGaugeMeasurementResolution _measurementResolution =
            RainGaugeMeasurementResolution.FiveMinutes;

        private Thread _measuringThread = new Thread(() => { });

        private readonly ArrayList
            _precipitation = new ArrayList(); //WindSpeeds during one measurement resolution unit in [mm * 10000]

        public SparkFunRainGaugeDriver(GpioController gpioController, GpioPins pin)
        {
            _gpioController = gpioController;
            _pin = _gpioController.OpenPin((int) pin);
            _pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _pulseCounter = new GpioChangeCounter(_pin) {Polarity = GpioChangePolarity.Rising};
        }

        public void StartMeasurement(RainGaugeMeasurementResolution measurementResolution)
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
                    Thread.Sleep((int) measurementResolution * 1000);
                    var count = _pulseCounter.Read();
                    _pulseCounter.Reset();
                    _precipitation.Add((uint)(count.Count / 2 * ONE_PULSE_PRECIPITATION));
                }

                _pulseCounter.Stop();
                _pulseCounter.Reset();
                _precipitation.Clear();
            });

            _measuringThread.Start();
        }

        public RainGaugeData GetData()
        {
            return new RainGaugeData(_measurementResolution, _precipitation.ToArray(typeof(uint)) as uint[]);
        }

        public void Reset()
        {
            _precipitation.Clear();
        }

        public void StopMeasurement()
        {
            _isMeasuring = false;
            _measuringThread.Join();
        }
    }
}