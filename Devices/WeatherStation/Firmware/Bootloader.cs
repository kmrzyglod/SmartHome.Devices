using System.Threading;
using WeatherStation.Infrastructure;
using WeatherStation.Infrastructure.Config;

namespace WeatherStation.Firmware
{
    internal static class Bootloader
    {
        public static void StartServices()
        {
            var defaultConfig = new WeatherStationConfiguration();
            var driversFactory = new DriversFactory(defaultConfig);
            //var servicesFactory = new ServiceFactory(driversFactory, defaultConfig)
              //  .InitWifi()
                //.InitMqttClient();
            //.InitInboundMessagesProcessing();

            var test = driversFactory.RainGaugeDriver;
        }
    }
}