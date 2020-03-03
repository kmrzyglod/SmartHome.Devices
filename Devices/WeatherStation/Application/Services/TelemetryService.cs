using System;
using System.Threading;
using EspIot.Application.Events.Outbound;
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
using nanoFramework.Runtime.Native;
using WeatherStation.Application.Commands.SetTelemetryInterval;
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
                    Debug.GC(false);
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
            if (interval < SetTelemetryIntervalCommand.MIN_INTERVAL)
            {
                throw new ArgumentOutOfRangeException(nameof(interval),
                    $"Interval must be greater or equal to {SetTelemetryIntervalCommand.MIN_INTERVAL} ms");
            }

            if (interval > SetTelemetryIntervalCommand.MAX_INTERVAL)
            {
                throw new ArgumentOutOfRangeException(nameof(interval),
                    $"Interval must be lower or equal to {SetTelemetryIntervalCommand.MAX_INTERVAL} ms");
            }

            _sentInterval = interval;
            _outboundEventBus.Send(new WeatherTelemetryIntervalChangedEvent(interval));
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

        private WeatherTelemetryEvent GetTelemetryData(DateTime measurementStartTime, DateTime measurementEndTime)
        {
            return new WeatherTelemetryEvent(measurementStartTime,
                measurementEndTime,
                GetTemperature(),
                GetPressure(),
                GetHumidity(),
                GetLightLevel(),
                _windVaneDriver.GetCurrentWindDirection(),
                _windVaneDriver.GetMostFrequentWindDirection(),
                _anemometerDriver.GetAverageWindSpeed() / 100f,
                _anemometerDriver.GetMaxWindSpeed() / 100f,
                _anemometerDriver.GetMinWindSpeed() / 100f,
                _rainGaugeDriver.GetPrecipitation() / 10000f);
        }

        private float GetTemperature()
        {
            try
            {
                return _bme280Driver.ReadTemperature();
            }
            catch (Exception e)
            {
                _outboundEventBus.Send(new ErrorEvent(
                    $"Error during temperature measurement: {e.Message}", ErrorLevel.Critical));
                return float.NaN;
            }
        }

        private float GetHumidity()
        {
            try
            {
                return _bme280Driver.ReadHumidity();
            }
            catch (Exception e)
            {
                _outboundEventBus.Send(new ErrorEvent(
                    $"Error during humidity measurement: {e.Message}", ErrorLevel.Critical));
                return float.NaN;
            }
        }

        private float GetPressure()
        {
            try
            {
                return _bme280Driver.ReadPreasure() / 100;
            }
            catch (Exception e)
            {
                _outboundEventBus.Send(new ErrorEvent(
                    $"Error during pressure measurement: {e.Message}", ErrorLevel.Critical));
                return float.NaN;
            }
        }

        private int GetLightLevel()
        {
            try
            {
                return _lightSensorDriver.GetLightLevelInLux();
            }
            catch (Exception e)
            {
                _outboundEventBus.Send(new ErrorEvent(
                    $"Error during light level measurement: {e.Message}", ErrorLevel.Critical));
                return int.MinValue;
            }
        }
    }
}