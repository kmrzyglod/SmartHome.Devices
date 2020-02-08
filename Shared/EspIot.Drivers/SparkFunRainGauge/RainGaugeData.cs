using System;
using EspIot.Drivers.SparkFunRainGauge.Enums;

namespace EspIot.Drivers.SparkFunRainGauge
{
    public class RainGaugeData
    {
        public RainGaugeData(RainGaugeMeasurementResolution measurementResolution, uint[] precipitation)
        {
            MeasurementResolution = measurementResolution;
            Precipitation = precipitation;
        }

        public RainGaugeMeasurementResolution MeasurementResolution { get; }
        public uint[] Precipitation { get; } //WindSpeeds during one measurement resolution unit in [mm * 10000]
    }
}