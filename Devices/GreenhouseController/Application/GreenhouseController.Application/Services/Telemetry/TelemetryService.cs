using System;
using System.Diagnostics;
using System.Threading;
using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;
using EspIot.Core.Messaging.Enum;
using EspIot.Drivers.Bh1750;
using EspIot.Drivers.Bme280;
using EspIot.Drivers.DfrobotSoilMoistureSensor;
using EspIot.Drivers.Switch;
using EspIot.Drivers.Switch.Enums;
using GreenhouseController.Application.Events.Outbound;

namespace GreenhouseController.Application.Services.Telemetry
{
    public class TelemetryService : IService
    {
        private readonly Bme280 _bme280Driver;
        private readonly SwitchDriver _doorReedSwitch;
        private readonly Bh1750 _lightSensorDriver;
        private readonly IOutboundEventBus _outboundEventBus;
        private readonly DfrobotSoilMoistureSensor _soilMoistureSensorDriver;
        private bool _isServiceRunning;
        private bool _runWorkingThread;
        private int _sentInterval = 600_000; //in [ms], default 10 min 
        private Thread _workingThread = new Thread(() => { });

        public TelemetryService(
            SwitchDriver doorReedSwitch,
            IOutboundEventBus outboundEventBus,
            Bh1750 lightSensorDriver,
            Bme280 bme280Driver,
            DfrobotSoilMoistureSensor soilMoistureSensorDriver)
        {
            _doorReedSwitch = doorReedSwitch;
            _outboundEventBus = outboundEventBus;
            _lightSensorDriver = lightSensorDriver;
            _bme280Driver = bme280Driver;
            _soilMoistureSensorDriver = soilMoistureSensorDriver;
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
                _outboundEventBus.Send(new DeviceStatusUpdatedEvent("Telemetry service was started",
                    DeviceStatusCode.ServiceStarted));
                while (_runWorkingThread)
                {
                    var measurementStartTime = DateTime.UtcNow;
                    Thread.Sleep(_sentInterval);
                    var measurementEndTime = DateTime.UtcNow;
                    var telemetryData = GetTelemetryData(measurementStartTime, measurementEndTime);
                    Debug.WriteLine($"Humidity={telemetryData.Humidity}, Temperature={telemetryData.Temperature}, LightLevel={telemetryData.LightLevel}, SoilMoisture={telemetryData.SoilMoisture}, IsDoorOpen={telemetryData.IsDoorOpen}");
                    _outboundEventBus.Send(telemetryData);
                }

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

        private GreenhouseControllerTelemetryEvent GetTelemetryData(DateTime measurementStartTime, DateTime measurementEndTime)
        {
            return new GreenhouseControllerTelemetryEvent(measurementStartTime,
                measurementEndTime,
                GetTemperature(),
                GetPressure(),
                GetHumidity(),
                GetLightLevel(),
                GetSoilMoisture(),
                _doorReedSwitch.GetState().ToBool());
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
                return _bme280Driver.ReadPreasure() / 100f;
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

        private short GetSoilMoisture()
        {
            try
            {
                return _soilMoistureSensorDriver.GetUncalibratedMoisture();
            }
            catch (Exception e)
            {
                _outboundEventBus.Send(new ErrorEvent(
                    $"Error during soil moisture measurement: {e.Message}", ErrorLevel.Critical));
                return short.MinValue;
            }
        }
    }
}