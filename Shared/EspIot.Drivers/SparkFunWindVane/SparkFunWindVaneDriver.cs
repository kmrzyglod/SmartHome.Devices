using Windows.Devices.Adc;

namespace EspIot.Drivers.SparkFunWindVane
{
    public class SparkFunWindVaneDriver
    {
        private readonly AdcChannel _adcChannel;
        private int ADC_SIGNAL_UNCERTAINTY = 200;

        public SparkFunWindVaneDriver(short adcChannel)
        {
            var adc = AdcController.GetDefault();
            _adcChannel = adc.OpenChannel(adcChannel);
        }

        public int GetRawData()
        {
            return _adcChannel.ReadValue();
        }
    }
}
