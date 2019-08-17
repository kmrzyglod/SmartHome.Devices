using EspIot.Drivers.Bh1750;
using EspIot.Drivers.Bme280;
using GreenhouseController.Config;

namespace GreenhouseController.Drivers
{
    class DriversFactory
    {
        private readonly GreenhouseControllerConfiguration _configuration;

        public DriversFactory(GreenhouseControllerConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Bh1750 LightSensor => new Bh1750(_configuration.Bh1750I2cController);
        public Bme280 Bme280 => new Bme280(_configuration.Bme280I2cController);
    }
}
