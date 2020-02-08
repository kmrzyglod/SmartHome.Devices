using System;
using System.Threading;
using EspIot.Drivers.SparkFunAnemometer.Enums;
using EspIot.Drivers.SparkFunRainGauge.Enums;
using WeatherStation.Infrastructure;
using WeatherStation.Infrastructure.Config;

namespace WeatherStation.Firmware
{
    internal static class Bootloader
    {
        public static void StartServices()
        {
            var defaultConfig = new WeatherStationConfiguration();
            var driversFactory = new DriversFactory(defaultConfig);
            var servicesFactory = new ServiceFactory(driversFactory, defaultConfig)
                  .InitWifi()
                  .InitMqttClient()
            .InitInboundMessagesProcessing();

            var test = driversFactory.RainGaugeDriver;
            var tes2 = driversFactory.AnemometerDriver;
            var bme280 = driversFactory.Bme280;
            var lightSensor = driversFactory.LightSensor;

            test.StartMeasurement(RainGaugeMeasurementResolution.OneMinute);
            tes2.StartMeasurement(AnemometerMeasurementResolution.FiveSeconds);
            
            while (true)
            {
                var temp = bme280.ReadTemperature();
                var humidity = bme280.ReadHumidity();
                var pressure = bme280.ReadPreasure();

                var light = lightSensor.GetLightLevelInLux();
                var data = test.GetData().Precipitation;
                var data2 = tes2.GetData().WindSpeeds; 
                Console.WriteLine($"Temp: {temp}, Humidity: {humidity } %, Pressure: {pressure} hPa");
                Console.WriteLine($"Light: {light} lx");
                //if (data.Length == 0)
                //{
                //    Thread.Sleep(10000);
                //    continue;
                //}
                //else
                //{
                //    Console.WriteLine($"Rain in measure {data.Length - 1}: { data[data.Length - 1] / 10000d}");

                //}

                if (data2.Length == 0)
                {
                    Thread.Sleep(5000);
                    continue;
                }
                else
                {
                    Console.WriteLine($"Wind speed {data2.Length - 1}: { data2[data2.Length - 1] / 100}");
                }

                Console.WriteLine("");
                Thread.Sleep(10000);
            }
            //var test2 = driversFactory.AnemometerDriver;

        }
    }
}