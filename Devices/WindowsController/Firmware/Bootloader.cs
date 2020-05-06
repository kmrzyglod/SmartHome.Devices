using EspIot.Core.Helpers;

namespace WindowsController.Firmware
{
    internal static class Bootloader
    {
        public static void StartServices()
        {
            Logger.Log(() => $"Free memory after started CLR {Debug.GC(false)}");
            var defaultConfig = new WeatherStationConfiguration();
            var driversFactory = new DriversFactory(defaultConfig);

            new ServiceFactory(driversFactory, defaultConfig)
                .InitWifi()
                .InitMqttClient()
                .InitDiagnosticService()
                .InitTelemetryService()
                .InitInboundMessagesProcessing();
        }
    }
}