using EspIot.Core.Gpio;
using EspIot.Drivers.Bh1750;
using EspIot.Drivers.Bme280;
using EspIot.Drivers.DfrobotSoilMoistureSensor;
using EspIot.Drivers.LinearActuator;
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

        public DriversFactory(GreenhouseControllerConfiguration configuration)
        {
            _configuration = configuration;
            
            //Configure I2C1 bus
            Configuration.SetPinFunction((int)_configuration.I2c1DataPin, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction((int)_configuration.I2c1ClockPin, DeviceFunction.I2C1_CLOCK);
            
            //Configure I2C2 bus
            Configuration.SetPinFunction((int)_configuration.I2c2DataPin, DeviceFunction.I2C2_DATA);
            Configuration.SetPinFunction((int)_configuration.I2c2ClockPin, DeviceFunction.I2C2_CLOCK);
        }

        public Bh1750 LightSensor => new Bh1750(_configuration.Bh1750I2cController);
        public Bme280 Bme280 => new Bme280(_configuration.Bme280I2cController).Initialize();
        public DfrobotSoilMoistureSensor SoilMoistureSensor => new DfrobotSoilMoistureSensor(_configuration.SoilMoistureSensorAdc);
        public WaterFlowSensor WaterFlowSensor => new WaterFlowSensor(GpioController.GetDefault(), _configuration.WaterFlowSensorPin, _configuration.WaterFlowSensorMeasurementTime);
        public LinearActuatorDriver Window1Actuator => new LinearActuatorDriver(GpioController.GetDefault(), _configuration.Window1ActuatorExtensionPin, _configuration.Window1ActuatorReductionPin, _configuration.Window1ActuatorMode);
        public LinearActuatorDriver Window2Actuator => new LinearActuatorDriver(GpioController.GetDefault(), _configuration.Window2ActuatorExtensionPin, _configuration.Window2ActuatorReductionPin, _configuration.Window2ActuatorMode);
        public ReedSwitchDriver DoorReedSwitch => new ReedSwitchDriver(GpioController.GetDefault(), _configuration.DoorReedSwitchPin);
        public ReedSwitchDriver Window1ReedSwitch => new ReedSwitchDriver(GpioController.GetDefault(), _configuration.Window1ReedSwitchPin);
        public ReedSwitchDriver Window2ReedSwitch => new ReedSwitchDriver(GpioController.GetDefault(), _configuration.Window2ReedSwitchPin);
        public StatusLed StatusLed => new StatusLed(GpioController.GetDefault(), _configuration.WifiStatusLed, _configuration.MqttStatusLed);
    }
}
