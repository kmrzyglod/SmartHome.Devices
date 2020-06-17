using Infrastructure.Config;
using Infrastructure.Factory;

namespace GreenhouseController
{
    internal static class Bootloader
    {
        public static void StartServices()
        {
            var defaultConfig = new Configuration();
            var driversFactory = new DriversFactory(defaultConfig);

            new ServiceFactory(driversFactory, defaultConfig)
                .InitWifi()
                .InitMqttClient()
                .InitDiagnosticService()
                .InitTelemetry()
                .InitInboundMessagesProcessing()
                .InitIrrigationService()
                .InitEnvironmentalConditionsService();

            driversFactory.SolidStateRelaysDriver.On(0);
        }
    }
}