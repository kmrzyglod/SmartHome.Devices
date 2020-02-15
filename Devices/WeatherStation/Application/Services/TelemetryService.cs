using System;
using System.Threading;
using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;
using EspIot.Drivers.Bh1750;
using EspIot.Drivers.Bme280;
using EspIot.Drivers.SparkFunAnemometer;
using EspIot.Drivers.SparkFunAnemometer.Enums;
using EspIot.Drivers.SparkFunRainGauge;
using EspIot.Drivers.SparkFunWindVane;
using EspIot.Drivers.SparkFunWindVane.Enums;
using WeatherStation.Application.Events.Outbound;

namespace WeatherStation.Application.Services
{
    public class TelemetryService : IService
    {
        private readonly SparkFunAnemometerDriver _anemometerDriver;
        private readonly Bme280 _bme280Driver;
        private readonly Bh1750 _lightSensorDriver;
        private readonly IOutboundEventBus _outboundEventBus;
        private readonly SparkFunRainGaugeDriver _rainGaugeDriver;
        private readonly SparkFunWindVaneDriver _windVaneDriver;
        private bool _isServiceRunning;
        private bool _runWorkingThread;
        private int _sentInterval = 300_000; //in [ms], default 5 min 
        private Thread _workingThread = new Thread(() => { });

        public TelemetryService(IOutboundEventBus outboundEventBus,
            SparkFunAnemometerDriver anemometerDriver,
            SparkFunWindVaneDriver windVaneDriver,
            SparkFunRainGaugeDriver rainGaugeDriver,
            Bme280 bme280Driver,
            Bh1750 lightSensorDriver)
        {
            _outboundEventBus = outboundEventBus;
            _anemometerDriver = anemometerDriver;
            _windVaneDriver = windVaneDriver;
            _rainGaugeDriver = rainGaugeDriver;
            _bme280Driver = bme280Driver;
            _lightSensorDriver = lightSensorDriver;
        }

        public bool IsRunning()
        {
            return _isServiceRunning;
        }

        public void Stop()
        {
            _runWorkingThread = false;
        }

        public void Start()
        {
            if (_isServiceRunning || _workingThread.IsAlive)
            {
                return;
            }

            _isServiceRunning = true;
            _runWorkingThread = true;

            _workingThread = new Thread(() =>
            {
                StartMeasurements();
                _outboundEventBus.Send(new DeviceStatusUpdatedEvent("Telemetry service was started",
                    DeviceStatusCode.ServiceStarted));
                while (_runWorkingThread)
                {
                    var measurementStartTime = DateTime.UtcNow;
                    Thread.Sleep(_sentInterval);
                    var measurementEndTime = DateTime.UtcNow;
                    var telemetryData = GetTelemetryData(measurementStartTime, measurementEndTime);
                    ResetMeasurements();
                    _outboundEventBus.Send(telemetryData);
                }

                StopMeasurements();
                _outboundEventBus.Send(new DeviceStatusUpdatedEvent("Telemetry service was stopped",
                    DeviceStatusCode.ServiceStopped));
                _isServiceRunning = false;
            });

            _workingThread.Start();
        }

        public void SetInterval(int interval)
        {
            _sentInterval = interval;
            _outboundEventBus.Send(new TelemetryIntervalChangedEvent(interval));
        }

        private void StartMeasurements()
        {
            _rainGaugeDriver.StartMeasurement();
            _anemometerDriver.StartMeasurement(AnemometerMeasurementResolution.FiveSeconds);
            _windVaneDriver.StartMeasurement(WindVaneMeasurementResolution.FiveSeconds);
        }

        private void StopMeasurements()
        {
            _rainGaugeDriver.StopMeasurement();
            _anemometerDriver.StopMeasurement();
            _windVaneDriver.StopMeasurement();
        }

        private void ResetMeasurements()
        {
            _rainGaugeDriver.Reset();
            _anemometerDriver.Reset();
            _windVaneDriver.Reset();
        }

        private TelemetryEvent GetTelemetryData(DateTime measurementStartTime, DateTime measurementEndTime)
        {
            float temperature = float.NaN;
            float humidity = float.NaN;
            float pressure = float.NaN;
            int lightLevel = int.MinValue;
            var currentWindDirection = WindDirection.Undefined;
            var mostFrequentWindDirection = WindDirection.Undefined;
            float averageWindSpeed = float.NaN;
            float maxWindSpeed = float.NaN;
            float minWindSpeed = float.NaN;
            float precipitation = float.NaN;

            try
            {
                temperature = _bme280Driver.ReadTemperature();
                humidity = _bme280Driver.ReadHumidity();
                pressure = _bme280Driver.ReadPreasure() / 100;
                lightLevel = _lightSensorDriver.GetLightLevelInLux();
                currentWindDirection = _windVaneDriver.GetCurrentWindDirection();
                mostFrequentWindDirection = _windVaneDriver.GetMostFrequentWindDirection();
                averageWindSpeed = _anemometerDriver.GetAverageWindSpeed() / 100f;
                maxWindSpeed = _anemometerDriver.GetMaxWindSpeed() / 100f;
                minWindSpeed = _anemometerDriver.GetMinWindSpeed() / 100f;
                precipitation = _rainGaugeDriver.GetPrecipitation() / 10000f;
            }
            catch (Exception e)
            {
                _outboundEventBus.Send(new ErrorEvent(Guid.NewGuid().ToString(),
                    $"Error during measurement: {e.Message}", ErrorLevel.Critical));
            }

            return new TelemetryEvent(measurementStartTime,
                measurementEndTime,
                temperature,
                pressure,
                humidity,
                lightLevel,
                currentWindDirection,
                mostFrequentWindDirection,
                averageWindSpeed,
                maxWindSpeed,
                minWindSpeed,
                precipitation);
        }
    }
}