using Windows.Devices.Gpio;
using WindowsController.Infrastructure.Config;
using WindowsController.Infrastructure.Factory;
using EspIot.Core.Helpers;
using nanoFramework.Runtime.Native;

namespace WindowsController.Firmware
{
    internal static class Bootloader
    {
        public static void StartServices()
        {
            Logger.Log(() => $"Free memory after started CLR {Debug.GC(false)}");
            var defaultConfig = new Configuration();
            var driversFactory = new DriversFactory(defaultConfig);

            new ServiceFactory(driversFactory, defaultConfig)
                .InitWifi()
                .InitMqttClient()
                .InitDiagnosticService()
                .InitWindowsManagingService()
                .InitInboundMessagesProcessing();

            //var gpioController = GpioController.GetDefault();
            //var pin  = gpioController.OpenPin((int)22);
            //pin.SetDriveMode(GpioPinDriveMode.Input);
            //pin.ValueChanged += PinValueChangedHandler;
        }

        private static void PinValueChangedHandler(object sender, GpioPinValueChangedEventArgs e)
        {
           Logger.Log(e.Edge.ToString);
        }

    }
}