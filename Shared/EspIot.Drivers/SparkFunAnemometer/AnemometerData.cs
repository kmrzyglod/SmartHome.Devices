using EspIot.Drivers.SparkFunAnemometer.Enums;
using EspIot.Drivers.SparkFunRainGauge.Enums;

namespace EspIot.Drivers.SparkFunAnemometer
{
    public class AnemometerData
    {
        public AnemometerData(AnemometerMeasurementResolution measurementResolution, uint[] windSpeeds)
        {
            MeasurementResolution = measurementResolution;
            WindSpeeds = windSpeeds;
        }

        public AnemometerMeasurementResolution MeasurementResolution { get; }
        public uint[] WindSpeeds { get; } //Average wind speeds during one measurement resolution unit in [m/s * 100]
    }
}
