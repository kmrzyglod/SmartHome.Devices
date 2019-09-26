using Windows.Devices.Adc;

namespace EspIot.Drivers.DfrobotSoilMoistureSensor
{
    public class DfrobotSoilMoistureSensor
    {
        private readonly AdcChannel _adcChannel;
        private const int SensorMaxValue = 3131;
        private const int SensorMinValue = 1440;

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
            short value = (short)(100 - ((_adcChannel.ReadValue() - SensorMinValue) / (float)(SensorMaxValue - SensorMinValue) * 100));
            return value < 0 ? (short) 0 : value > 100 ? (short)100 : value;
        }
    }
}
