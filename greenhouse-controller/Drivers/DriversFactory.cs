using EspIot.Drivers.Bh1750;
using EspIot.Drivers.Bme280;
using EspIot.Drivers.DfrobotSoilMoistureSensor;
using EspIot.Drivers.LinearActuator;
using EspIot.Drivers.Mqtt;
using EspIot.Drivers.ReedSwitch;
using EspIot.Drivers.SeedstudioWaterFlowSensor;
using EspIot.Drivers.StatusLed;
using GreenhouseController.Config;
using nanoFramework.Hardware.Esp32;
using Windows.Devices.Gpio;

namespace GreenhouseController.Drivers
{
    class DriversFactory
    {
        private readonly GreenhouseControllerConfiguration _configuration;
        
        private Bh1750 _lightSensor;
        private Bme280 _bme280;
        private DfrobotSoilMoistureSensor _soilMoistureSensor;
        private WaterFlowSensor _waterFlowSensor;
        private LinearActuatorDriver _window1Actuator;
        private LinearActuatorDriver _window2Actuator;
        private ReedSwitchDriver _doorReedSwitch;
        private ReedSwitchDriver _window1ReedSwitch;
        private ReedSwitchDriver _window2ReedSwitch;
        private StatusLed _statusLed;
        private MqttClientWrapper _iotHubClient;
        
        public DriversFactory(GreenhouseControllerConfiguration configuration)
        {
            _configuration = configuration;

            //Configure I2C1 bus
            Configuration.SetPinFunction((int)_configuration.I2C1DataPin, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction((int)_configuration.I2C1ClockPin, DeviceFunction.I2C1_CLOCK);

            //Configure I2C2 bus
            Configuration.SetPinFunction((int)_configuration.I2C2DataPin, DeviceFunction.I2C2_DATA);
            Configuration.SetPinFunction((int)_configuration.I2C2ClockPin, DeviceFunction.I2C2_CLOCK);
        }

        public Bh1750 LightSensor => _lightSensor ?? (_lightSensor = new Bh1750(_configuration.Bh1750I2CController));
        public Bme280 Bme280 => _bme280 ?? (_bme280 = new Bme280(_configuration.Bme280I2CController).Initialize());
        public DfrobotSoilMoistureSensor SoilMoistureSensor => _soilMoistureSensor ?? (_soilMoistureSensor = new DfrobotSoilMoistureSensor(_configuration.SoilMoistureSensorAdc));
        public WaterFlowSensor WaterFlowSensor => _waterFlowSensor ?? (_waterFlowSensor = new WaterFlowSensor(GpioController.GetDefault(), _configuration.WaterFlowSensorPin, _configuration.WaterFlowSensorMeasurementTime));
        public LinearActuatorDriver Window1Actuator => _window1Actuator ?? (_window1Actuator =  new LinearActuatorDriver(GpioController.GetDefault(), _configuration.Window1ActuatorExtensionPin, _configuration.Window1ActuatorReductionPin, _configuration.Window1ActuatorMode));
        public LinearActuatorDriver Window2Actuator => _window2Actuator ?? (_window2Actuator = new LinearActuatorDriver(GpioController.GetDefault(), _configuration.Window2ActuatorExtensionPin, _configuration.Window2ActuatorReductionPin, _configuration.Window2ActuatorMode));
        public ReedSwitchDriver DoorReedSwitch => _doorReedSwitch ?? (_doorReedSwitch = new ReedSwitchDriver(GpioController.GetDefault(), _configuration.DoorReedSwitchPin));
        public ReedSwitchDriver Window1ReedSwitch => _window1ReedSwitch ?? (_window1ReedSwitch = new ReedSwitchDriver(GpioController.GetDefault(), _configuration.Window1ReedSwitchPin));
        public ReedSwitchDriver Window2ReedSwitch => _window2ReedSwitch ?? (_window2ReedSwitch = new ReedSwitchDriver(GpioController.GetDefault(), _configuration.Window2ReedSwitchPin));
        public StatusLed StatusLed => _statusLed ?? (_statusLed = new StatusLed(GpioController.GetDefault(), _configuration.WifiStatusLed, _configuration.MqttStatusLed));
        public MqttClientWrapper IotHubClient => _iotHubClient ?? (_iotHubClient = new MqttClientWrapper(_configuration.MqttBrokerAddress, _configuration.DeviceId));
    }
}
