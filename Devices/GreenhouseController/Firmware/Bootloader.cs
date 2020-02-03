using Infrastructure.Config;
using Infrastructure.Factory;

namespace GreenhouseController
{
    internal static class Bootloader
    {
        public static void StartServices()
        {
            var defaultConfig = new GreenhouseControllerConfiguration();
            var driversFactory = new DriversFactory(defaultConfig);
            var servicesFactory = new ServiceFactory(driversFactory, defaultConfig)
                .InitWifi()
                .InitMqttClient()
                .InitTelemetry()
                .InitWindowsManager()
                .InitInboundMessagesProcessing();
        }
    }
}