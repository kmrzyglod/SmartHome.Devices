using System;
using System.Threading;
using Windows.Devices.Gpio;
using EspIot.Core.Gpio;
using EspIot.Core.Helpers;
using EspIot.Drivers.SeedstudioWaterFlowSensor.Enums;

namespace EspIot.Drivers.SeedstudioWaterFlowSensor
{
    public class WaterFlowSensorDriver
    {
        private const uint MAX_REAL_FLOW = 150; //30 l/min -> 150 [l/min * 5]
        private readonly GpioController _gpioController;
        private readonly GpioPin _pin;
        private readonly GpioChangeCounter _pulseCounter;

        private uint _averageFlow;
        private uint _currentFlow;
        private bool _isMeasuring;
        private uint _maxFlow;
        private uint _measurementCounter;
        private Thread _measuringThread = new Thread(() => { });
        private uint _minFlow = uint.MaxValue;
        private uint _totalFlow; 

        public WaterFlowSensorDriver(GpioController gpioController, GpioPins pin)
        {
            _gpioController = gpioController;
            _pin = _gpioController.OpenPin((int) pin);
            _pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _pulseCounter = new GpioChangeCounter(_pin) {Polarity = GpioChangePolarity.Rising};
        }


        public void StartMeasurement(WaterFlowSensorMeasurementResolution measurementResolution)
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
                    Thread.Sleep((int) measurementResolution * 1000);
                    var pulseCount = _pulseCounter.Read();
                    _pulseCounter.Reset();
                    _currentFlow =
                        (ushort) (pulseCount.Count / (double) measurementResolution);

                    if (_currentFlow > MAX_REAL_FLOW)
                    {
                        continue;
                    }

                    _totalFlow += (ushort) pulseCount.Count;

                    _averageFlow = (_averageFlow * _measurementCounter + _currentFlow) /
                                   ++_measurementCounter;

                    //Logger.Log(() => $"Current flow: {_currentFlow / 5f} [l/min]");
                    //Logger.Log(() => $"Total flow: {_totalFlow / 5f / 60f} [l]");

                    if (_currentFlow > _maxFlow)
                    {
                        _maxFlow = _currentFlow;
                    }

                    if (_currentFlow < _minFlow)
                    {
                        _minFlow = _currentFlow;
                    }
                }

                _pulseCounter.Stop();
                _pulseCounter.Reset();
            });

            _measuringThread.Start();
        }

        public float GetCurrentFlow()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return _currentFlow / 5f;
        }

        public float GetAverageFlow()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return _averageFlow / 5f;
        }

        public float GetMaxFlow()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return _maxFlow / 5f;
        }

        public float GetMinFlow()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return _minFlow / 5f;
        }

        public float GetTotalFlow()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return _totalFlow / 5f / 60f;
        }

        public void Reset()
        {
            _measurementCounter = 0;
            _maxFlow = 0;
            _averageFlow = 0;
            _minFlow = uint.MaxValue;
            _totalFlow = 0;
        }

        public void StopMeasurement()
        {
            _isMeasuring = false;
            _measuringThread.Join();
            Reset();
        }
    }
}