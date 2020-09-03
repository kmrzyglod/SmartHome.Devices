using System;
using System.Diagnostics;
using Windows.Devices.Gpio;
using WindowsController.Infrastructure.Config;
using WindowsController.Infrastructure.Factory;
using EspIot.Core.Gpio;
using EspIot.Core.Helpers;
using GC = nanoFramework.Runtime.Native.GC;

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
                .InitTelemetryService()
                .InitInboundMessagesProcessing();

        }
    }
}