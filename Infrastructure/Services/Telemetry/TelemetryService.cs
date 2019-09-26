using EspIot.Drivers.Bh1750;
using EspIot.Drivers.Bme280;
using EspIot.Drivers.DfrobotSoilMoistureSensor;
using EspIot.Drivers.ReedSwitch;
using EspIot.Drivers.ReedSwitch.Enums;
using EspIot.Drivers.SeedstudioWaterFlowSensor;
using Infrastructure.Events.Outbound;
using Infrastructure.Telemetry;
using System.Threading;

namespace Infrastructure.Services.Telemetry
{
    public class TelemetryService
    {
        private readonly ReedSwitchDriver _doorReedSwitch;
        private readonly ReedSwitchDriver _window1ReedSwitch;
        private readonly ReedSwitchDriver _window2ReedSwitch;
        private readonly MqttOutboundEventBus _outboundEventBus;
        private readonly Bh1750 _lightSensorDriver;
        private readonly Bme280 _bme280Driver;
        private readonly DfrobotSoilMoistureSensor _soilMoistureSensorDriver;
        private readonly WaterFlowSensor _waterFlowSensor;
        private int _sentInterval = 600_000; //in [ms], default 10 min 
        private bool _isServiceActive = false;

        public TelemetryService(
            ReedSwitchDriver doorReedSwitch,
            ReedSwitchDriver window1ReedSwitch,
            ReedSwitchDriver window2ReedSwitch,
            MqttOutboundEventBus outboundEventBus,
            Bh1750 lightSensorDriver,
            Bme280 bme280Driver,
            DfrobotSoilMoistureSensor soilMoistureSensorDriver,
            WaterFlowSensor waterFlowSensor)
        {
            _doorReedSwitch = doorReedSwitch;
            _window1ReedSwitch = window1ReedSwitch;
            _window2ReedSwitch = window2ReedSwitch;
            _outboundEventBus = outboundEventBus;
            _lightSensorDriver = lightSensorDriver;
            _bme280Driver = bme280Driver;
            _soilMoistureSensorDriver = soilMoistureSensorDriver;
            _waterFlowSensor = waterFlowSensor;
        }

        public void SetInterval(int interval)
        {
            _sentInterval = interval;
        }

        public bool IsActive()
        {
            return _isServiceActive;
        }

        public void Start()
        {
            lock (this)
            {
                if (_isServiceActive)
                {
                    return;
                }

                _isServiceActive = true;
                
                new Thread(() =>
                {
                    while (_isServiceActive)
                    {
                        _outboundEventBus.Send(GetTelemetryData());
                        Thread.Sleep(_sentInterval);
                    }
                }).Start();
            }
        }

        public void Stop()
        {
            _isServiceActive = false;
        }

        private StatusMessage GetTelemetryData()
        {
            return new StatusMessage(
                _bme280Driver.ReadTemperature(),
                _bme280Driver.ReadPreasure(),
                _bme280Driver.ReadHumidity(),
                _lightSensorDriver.GetLightLevelInLux(),
                _soilMoistureSensorDriver.GetUncalibratedMoisture(),
                _waterFlowSensor.GetMomentaryFlowValue(),
                _doorReedSwitch.GetState() == ReedShiftState.Opened ? true : false,
                _window1ReedSwitch.GetState() == ReedShiftState.Opened ? true : false,
                _window2ReedSwitch.GetState() == ReedShiftState.Opened ? true : false
                );
        }
    }
}
