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
            Logger.Log(() => $"Free memory after started CLR {GC.Run(false)}");

            var defaultConfig = new Configuration();
            var driversFactory = new DriversFactory(defaultConfig);

            new ServiceFactory(driversFactory, defaultConfig)
                .InitWifi()
                .InitMqttClient()
                .InitDiagnosticService()
                .InitWindowsManagingService()
                .InitInboundMessagesProcessing();

            //var gpioController = GpioController.GetDefault();
            //var pin = gpioController.OpenPin((int)21);
            //pin.DebounceTimeout = TimeSpan.FromMilliseconds(20);
            //pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            //pin.ValueChanged += PinValueChangedHandler;
        }

        private static void PinValueChangedHandler(object sender, GpioPinValueChangedEventArgs e)
        {
           Logger.Log(e.Edge.ToString);
        }

    }
}