using System;
using System.Threading;
using Windows.Devices.Gpio;
using EspIot.Core.Gpio;
using EspIot.Core.Helpers;
using EspIot.Drivers.SparkFunAnemometer.Enums;

namespace EspIot.Drivers.SparkFunAnemometer
{
    //https://cdn.sparkfun.com/assets/8/4/c/d/6/Weather_Sensor_Assembly_Updated.pdf
    public class SparkFunAnemometerDriver
    {
        private const double ONE_HZ_PULSE_SPEED = 0.335d; // [m/s]
        private const double MAX_REAL_WIND_SPEED = 40d; //40 [m/s] -> 144 km/h
        private readonly GpioController _gpioController;

        private readonly GpioPin _pin;
        private readonly GpioChangeCounter _pulseCounter;

        //[m/s * 100]
        private double _averageWindSpeed;
        private double _currentWindSpeed;
        private double _minWindSpeed = uint.MaxValue;
        private double _maxWindSpeed = 0;
        private uint _measurementCounter;
        private bool _isMeasuring;

        private Thread _measuringThread = new Thread(() => { });
       

        public SparkFunAnemometerDriver(GpioController gpioController, GpioPins pin)
        {
            _gpioController = gpioController;
            _pin = _gpioController.OpenPin((int) pin);
            _pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _pulseCounter = new GpioChangeCounter(_pin) {Polarity = GpioChangePolarity.Falling};
        }

        public void StartMeasurement(AnemometerMeasurementResolution measurementResolution)
        {
            if (_measuringThread.IsAlive)
            {
                //don't start new measuring when previous is pending
                throw new InvalidOperationException(
                    "Cannot start measurement because another measurement process is in progress.");
            }

            _isMeasuring = true;

            _measuringThread = new Thread(() =>
            {
                _pulseCounter.Start();

                while (_isMeasuring)
                {
                    _pulseCounter.Reset();
                    Thread.Sleep((int) measurementResolution * 1000);
                    var pulseCount = _pulseCounter.Read();
                   
                    _currentWindSpeed = (pulseCount.Count / (double) measurementResolution * ONE_HZ_PULSE_SPEED);

                    if (_currentWindSpeed > MAX_REAL_WIND_SPEED)
                    {
                        continue;
                    }
                    
                    _averageWindSpeed = (_averageWindSpeed * _measurementCounter + _currentWindSpeed) /
                                        ++_measurementCounter;

                    Logger.Log(() => $"Current wind tick: {pulseCount.Count}");
                    Logger.Log(() => $"Current wind speed: {_currentWindSpeed } [m/s]");

                    if (_currentWindSpeed > _maxWindSpeed)
                    {
                        _maxWindSpeed = _currentWindSpeed;
                    }

                    if (_currentWindSpeed < _minWindSpeed)
                    {
                        _minWindSpeed = _currentWindSpeed;
                    }
                }

                _pulseCounter.Stop();
                _pulseCounter.Reset();
            });

            _measuringThread.Start();
        }

        public double GetCurrentWindSpeed()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return _currentWindSpeed;
        }

        public double GetAverageWindSpeed()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return _averageWindSpeed;
        }

        public double GetMaxWindSpeed()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return _maxWindSpeed;
        }

        public double GetMinWindSpeed()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return _minWindSpeed;
        }

        public void Reset()
        {
            _measurementCounter = 0;
            _maxWindSpeed = 0;
            _averageWindSpeed = 0;
            _minWindSpeed = uint.MaxValue;
        }

        public void StopMeasurement()
        {
            _isMeasuring = false;
            _measuringThread.Join();
            Reset();
        }
    }
}