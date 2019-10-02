using GreenhouseController.Drivers;

namespace GreenhouseController
{
    static class Bootloader
    {
        public static void StartServices()
        {
            var defaultConfig = new Config.GreenhouseControllerConfiguration();
            var driversFactory = new DriversFactory(defaultConfig);
            //var servicesFactory = new ServiceFactory(driversFactory, defaultConfig)
             //   .InitWifi()
              //  .InitMqttClient();
            //.InitTelemetry();
            //.InitWindowsManager();
        }
    }
}
