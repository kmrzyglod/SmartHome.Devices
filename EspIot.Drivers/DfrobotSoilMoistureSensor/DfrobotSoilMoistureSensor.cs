using Windows.Devices.Adc;

namespace EspIot.Drivers.DfrobotSoilMoistureSensor
{
    public class DfrobotSoilMoistureSensor
    {
        private readonly AdcChannel _adcChannel;
        private const int SENSOR_MAX_VALUE = 3131;
        private const int SENSOR_MIN_VALUE = 1440;

        public DfrobotSoilMoistureSensor(short adcChannel)
        {
            var adc1 = AdcController.GetDefault();
            _adcChannel = adc1.OpenChannel(adcChannel);
        }

        public int GetRawData()
        {
            return _adcChannel.ReadValue();
        }

        public short GetUncalibratedMoisture()
        {
            short value = (short)(100 - ((_adcChannel.ReadValue() - SENSOR_MIN_VALUE) / (float)(SENSOR_MAX_VALUE - SENSOR_MIN_VALUE) * 100));
            return value < 0 ? (short) 0 : value > 100 ? (short)100 : value;
        }
    }
}
