using EspIot.Core.Gpio;
using EspIot.Core.I2c;
using EspIot.Drivers.Bh1750;
using EspIot.Drivers.Bme280;
using EspIot.Drivers.DfrobotSoilMoistureSensor;
using EspIot.Drivers.LinearActuator;
using EspIot.Drivers.Mqtt;
using EspIot.Drivers.ReedSwitch;
using EspIot.Drivers.SeedstudioWaterFlowSensor;
using EspIot.Drivers.SoildStateRelay;
using EspIot.Drivers.StatusLed;
using EspIot.Drivers.Wifi;
using Json.NetMF;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Runtime.Native;
using System;
using System.Collections;
using System.Net.NetworkInformation;
using System.Threading;
using Windows.Devices.Adc;
using Windows.Devices.Gpio;

namespace GreenhouseController
{
    public class Program
    {
        public static void Main()
        {
            Bootloader.StartServices();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
