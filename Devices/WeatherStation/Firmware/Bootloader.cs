using System;
using System.Diagnostics;
using System.Threading;
using Windows.Devices.Gpio;
using EspIot.Core.Helpers;
using WeatherStation.Infrastructure.Config;
using WeatherStation.Infrastructure.Factory;
using GC = nanoFramework.Runtime.Native.GC;

namespace WeatherStation.Firmware
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
                .InitTelemetryService()
                .InitInboundMessagesProcessing();
        }
    }
}