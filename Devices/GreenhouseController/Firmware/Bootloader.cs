using EspIot.Infrastructure.Mappers;
using EspIot.Infrastructure.MessageBus;
using GreenhouseController.Application.Commands;
using Infrastructure.Config;
using Infrastructure.Factory;

namespace GreenhouseController
{
    static class Bootloader
    {
        public static void StartServices()
        {
            var defaultConfig = new GreenhouseControllerConfiguration();
            var driversFactory = new DriversFactory(defaultConfig);
            var servicesFactory = new ServiceFactory(driversFactory, defaultConfig)
                .InitWifi()
                .InitMqttClient();
            //.InitTelemetry();
            //.InitWindowsManager();

            var commandsFactory = new InboundMessagesMapper(driversFactory.IotHubClient, new CommandBus(), new CommandsFactory());

        }
    }
}
