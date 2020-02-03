using Windows.Devices.Gpio;
using EspIot.Drivers.Bh1750;
using EspIot.Drivers.Bme280;
using EspIot.Drivers.DfrobotSoilMoistureSensor;
using EspIot.Drivers.LinearActuator;
using EspIot.Drivers.ReedSwitch;
using EspIot.Drivers.SeedstudioWaterFlowSensor;
using EspIot.Drivers.SparkFunRainGauge;
using EspIot.Drivers.StatusLed;
using EspIot.Infrastructure.Mqtt;
using nanoFramework.Hardware.Esp32;
using WeatherStation.Infrastructure.Config;

namespace WeatherStation.Infrastructure
{
    public class DriversFactory
    {
        private readonly WeatherStationConfiguration _configuration;
        
        private Bh1750 _lightSensor;
        private Bme280 _bme280;
        private StatusLed _statusLed;
        private MqttClientWrapper _iotHubClient;
        private SparkFunRainGaugeDriver _rainGaugeDriver;
        
        public DriversFactory(WeatherStationConfiguration configuration)
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
        public StatusLed StatusLed => _statusLed ?? (_statusLed = new StatusLed(GpioController.GetDefault(), _configuration.WifiStatusLed, _configuration.MqttStatusLed));
        public MqttClientWrapper IotHubClient => _iotHubClient ?? (_iotHubClient = new MqttClientWrapper(_configuration.MqttBrokerAddress, _configuration.DeviceId));
        public SparkFunRainGaugeDriver RainGaugeDriver => _rainGaugeDriver ?? (_rainGaugeDriver = new SparkFunRainGaugeDriver(GpioController.GetDefault(), _configuration.RainGaugePin));
    }
}
