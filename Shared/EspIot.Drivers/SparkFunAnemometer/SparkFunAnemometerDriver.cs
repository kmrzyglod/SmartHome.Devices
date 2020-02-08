using System;
using System.Collections;
using System.Threading;
using Windows.Devices.Gpio;
using EspIot.Core.Extensions;
using EspIot.Core.Gpio;
using EspIot.Drivers.SparkFunAnemometer.Enums;

namespace EspIot.Drivers.SparkFunAnemometer
{
    public class SparkFunAnemometerDriver
    {
        //https://cdn.sparkfun.com/assets/8/4/c/d/6/Weather_Sensor_Assembly_Updated.pdf
        private const uint ONE_HZ_PULSE_SPEED = 67; // 2,4 km/h -> 0,67 [m/s] -> 67 [m/s * 100];
        private readonly GpioController _gpioController;

        private readonly AnemometerMeasurementResolution _measurementResolution =
            AnemometerMeasurementResolution.FiveSeconds;

        private readonly GpioPin _pin;
        private readonly GpioChangeCounter _pulseCounter;

        private readonly ArrayList
            _windSpeeds = new ArrayList(); //WindSpeeds during one measurement resolution unit in [mm * 10000]

        private bool _isMeasuring;

        private Thread _measuringThread = new Thread(() => { });

        public SparkFunAnemometerDriver(GpioController gpioController, GpioPins pin)
        {
            _gpioController = gpioController;
            _pin = _gpioController.OpenPin((int) pin);
            _pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _pulseCounter = new GpioChangeCounter(_pin) {Polarity = GpioChangePolarity.Rising};
        }

        public void StartMeasurement(AnemometerMeasurementResolution measurementResolution)
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
                    var pulseCount = _pulseCounter.Read();
                    _pulseCounter.Reset();
                    _windSpeeds.Add((uint)(pulseCount.Count / (double) measurementResolution * ONE_HZ_PULSE_SPEED));
                }

                _pulseCounter.Stop();
                _pulseCounter.Reset();
                _windSpeeds.Clear();
            });

            _measuringThread.Start();
        }

        public AnemometerData GetData()
        {
            return new AnemometerData(_measurementResolution, _windSpeeds.ToArray(typeof(uint)) as uint[]);
        }

        public void Reset()
        {
            _windSpeeds.Clear();
        }

        public void StopMeasurement()
        {
            _isMeasuring = false;
            _measuringThread.Join();
        }
    }
}