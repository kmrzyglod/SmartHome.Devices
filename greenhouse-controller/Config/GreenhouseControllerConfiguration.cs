using EspIot.Core.Configuration;
using EspIot.Core.Gpio;
using EspIot.Core.I2c;
using EspIot.Drivers.LinearActuator;
using System.Net.NetworkInformation;

namespace GreenhouseController.Config
{
    class GreenhouseControllerConfiguration : IConfiguration
    {
        //General config
        //WIFI config
        public string NetworkSsid { get; } = "";
        public string NetworkPassword { get; } = "";
        public Wireless80211Configuration.AuthenticationType NetworkAuthType { get; } = Wireless80211Configuration.AuthenticationType.WPA;
        public Wireless80211Configuration.EncryptionType NetworkEncryptionType { get; } = Wireless80211Configuration.EncryptionType.WPA_PSK;

        public string MqttBrokerAddress { get; } = "192.168.2.108";
        public string DeviceId { get; } = "esp32-greenhouse";

        //I2C bus config
        public GpioPins I2c1DataPin { get; } = GpioPins.GPIO_NUM_5;
        public GpioPins I2c1ClockPin { get; } = GpioPins.GPIO_NUM_18;

        public GpioPins I2c2DataPin { get; } = GpioPins.GPIO_NUM_19;
        public GpioPins I2c2ClockPin { get; } = GpioPins.GPIO_NUM_21;

        //Status led config
        public GpioPins WifiStatusLed { get; } = GpioPins.GPIO_NUM_23;
        public GpioPins MqttStatusLed { get; } = GpioPins.GPIO_NUM_22;

        //bme280 
        public string Bme280I2cController { get; } = I2cControllerName.I2C1;

        //bh1750 
        public string Bh1750I2cController { get; } = I2cControllerName.I2C2;

        //Soil moisture
        public short SoilMoistureSensorAdc { get; } = 0;

        //Reed switches
        public GpioPins DoorReedSwitchPin { get; } = GpioPins.GPIO_NUM_15;
        public GpioPins Window1ReedSwitchPin { get; } = GpioPins.GPIO_NUM_2;
        public GpioPins Window2ReedSwitchPin { get; } = GpioPins.GPIO_NUM_4;

        //Water flow sensor
        public GpioPins WaterFlowSensorPin { get; } = GpioPins.GPIO_NUM_39;
        public int WaterFlowSensorMeasurementTime { get; } = 1000;

        //Windows actuators
        public GpioPins Window1ActuatorExtensionPin { get; } = GpioPins.GPIO_NUM_15;
        public GpioPins Window1ActuatorReductionPin { get; } = GpioPins.GPIO_NUM_2;
        public Mode Window1ActuatorMode { get; } = Mode.DefaultHighState;

        //Windows actuators
        public GpioPins Window2ActuatorExtensionPin { get; } = GpioPins.GPIO_NUM_4;
        public GpioPins Window2ActuatorReductionPin { get; } = GpioPins.GPIO_NUM_2;
        public Mode Window2ActuatorMode { get; } = Mode.DefaultHighState;


    }
}
