using GreenhouseController.Commands;
using GreenhouseController.Drivers;
using GreenhouseController.Services;
using Infrastructure.Commands;

namespace GreenhouseController
{
    static class Bootloader
    {
        public static void StartServices()
        {
            var defaultConfig = new Config.GreenhouseControllerConfiguration();
            var driversFactory = new DriversFactory(defaultConfig);
            var servicesFactory = new ServiceFactory(driversFactory, defaultConfig)
                .InitWifi()
                .InitMqttClient();
            //.InitTelemetry();
            //.InitWindowsManager();

            var commandsFactory = new CommandsInput(driversFactory.IotHubClient, new CommandBus());

        }
    }
}
