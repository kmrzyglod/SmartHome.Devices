using EspIot.Core.Configuration;
using EspIot.Core.Gpio;
using EspIot.Core.I2c;
using EspIot.Drivers.LinearActuator;

namespace Infrastructure.Config
{
    public class Configuration : IConfiguration
    {
        //General config
        //MQTT config
        public string MqttBrokerAddress { get; } = "192.168.2.108";
        public string DeviceId { get; } = "esp32-greenhouse";
        public string InboundMessagesTopic { get; } = "devices/esp32-greenhouse/messages/devicebound/#";

        //I2C bus config
        public GpioPins I2C1DataPin { get; } = GpioPins.GPIO_NUM_18;
        public GpioPins I2C1ClockPin { get; } = GpioPins.GPIO_NUM_5;

        public GpioPins I2C2DataPin { get; } = GpioPins.GPIO_NUM_19;
        public GpioPins I2C2ClockPin { get; } = GpioPins.GPIO_NUM_21;

        //Status led config
        public GpioPins WifiStatusLed { get; } = GpioPins.GPIO_NUM_23;
        public GpioPins MqttStatusLed { get; } = GpioPins.GPIO_NUM_4;

        //bme280 
        public string Bme280I2CController { get; } = I2CControllerName.I2C1;

        //bh1750 
        public string Bh1750I2CController { get; } = I2CControllerName.I2C2;

        //Soil moisture
        public short SoilMoistureSensorAdc { get; } = 0; //GPIO_36

        //Reed switches
        public GpioPins DoorReedSwitchPin { get; } = GpioPins.GPIO_NUM_15;

        //Water flow sensor
        public GpioPins WaterFlowSensorPin { get; } = GpioPins.GPIO_NUM_2;

        //8 channel solid state relay pins
        public GpioPins[] SolidStateRelayPins { get; } = {
            GpioPins.GPIO_NUM_13,
            GpioPins.GPIO_NUM_12,
            GpioPins.GPIO_NUM_14,
            GpioPins.GPIO_NUM_27,
            GpioPins.GPIO_NUM_26,
            GpioPins.GPIO_NUM_25,
            GpioPins.GPIO_NUM_33,
            GpioPins.GPIO_NUM_32
        };

        //Pump channel 
        public short WaterPumpRelaySwitchChannel = 0;
    }
}
