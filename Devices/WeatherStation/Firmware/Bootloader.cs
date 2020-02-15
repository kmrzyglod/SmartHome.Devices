using WeatherStation.Infrastructure.Config;
using WeatherStation.Infrastructure.Factory;

namespace WeatherStation.Firmware
{
    internal static class Bootloader
    {
        public static void StartServices()
        {
            var defaultConfig = new WeatherStationConfiguration();
            var driversFactory = new DriversFactory(defaultConfig);

            new ServiceFactory(driversFactory, defaultConfig)
                .InitWifi()
                .InitMqttClient()
                .InitTelemetryService()
                .InitInboundMessagesProcessing();
        }
    }
}