﻿using EspIot.Core.Configuration;
using EspIot.Core.Gpio;
using EspIot.Core.I2c;

namespace WeatherStation.Infrastructure.Config
{
    public class Configuration : IConfiguration
    {
        //General config
        //MQTT config
        public string MqttBrokerAddress { get; } = "192.168.3.8";
        public string DeviceId { get; } = "esp32-weather-station";
        public string InboundMessagesTopic { get; } = "devices/esp32-weather-station/messages/devicebound/#";

        //I2C bus config
        public GpioPins I2C1DataPin { get; } = GpioPins.GPIO_NUM_18;
        public GpioPins I2C1ClockPin { get; } = GpioPins.GPIO_NUM_5;

        public GpioPins I2C2DataPin { get; } = GpioPins.GPIO_NUM_19;
        public GpioPins I2C2ClockPin { get; } = GpioPins.GPIO_NUM_21;

        //Status led config
        public GpioPins WifiStatusLed { get; } = GpioPins.GPIO_NUM_32;
        public GpioPins MqttStatusLed { get; } = GpioPins.GPIO_NUM_33;

        //bme280 
        public string Bme280I2CController { get; } = I2CControllerName.I2C1;

        //bh1750 
        public string Bh1750I2CController { get; } = I2CControllerName.I2C2;

        //Rain gauge
        public GpioPins RainGaugePin { get; } = GpioPins.GPIO_NUM_23;
        
        //Anemometer
        public GpioPins AnemometerPin { get; } = GpioPins.GPIO_NUM_4;

        //Wind Vane
        public ushort WindVaneAdcChannel { get; } = 0;
    }
}
