using System;
using System.Threading;
using Windows.Devices.Adc;
using EspIot.Core.Helpers;
using EspIot.Drivers.SparkFunWindVane.Enums;

namespace EspIot.Drivers.SparkFunWindVane
{
    //https://cdn.sparkfun.com/assets/8/4/c/d/6/Weather_Sensor_Assembly_Updated.pdf
    public class SparkFunWindVaneDriver
    {
        private readonly AdcChannel _adcChannel;
        private readonly ushort[] _windDirectionCounters = new ushort[8];
        private readonly ushort ADC_SIGNAL_UNCERTAINTY = 200;
        private WindDirection _currentWindDirection;
        private bool _isMeasuring;
        private Thread _measuringThread = new Thread(() => { });

        public SparkFunWindVaneDriver(ushort adcChannel)
        {
            var adc = AdcController.GetDefault();
            _adcChannel = adc.OpenChannel(adcChannel);
        }

        public void StartMeasurement(WindVaneMeasurementResolution measurementResolution)
        {
            if (_measuringThread.IsAlive)
            {
                throw new InvalidOperationException(
                    "Cannot start measurement because another measurement process is in progress.");
            }

            _isMeasuring = true;

            _measuringThread = new Thread(() =>
            {
                while (_isMeasuring)
                {
                    ushort adcVal = (ushort) _adcChannel.ReadValue();
                    if (adcVal >= (ushort) WindDirectionAdcResponse.N - ADC_SIGNAL_UNCERTAINTY &&
                        adcVal <= (ushort) WindDirectionAdcResponse.N + ADC_SIGNAL_UNCERTAINTY)
                    {
                        _windDirectionCounters[(ushort) WindDirection.N]++;
                        _currentWindDirection = WindDirection.N;
                    }

                    else if (adcVal >= (ushort) WindDirectionAdcResponse.NE - ADC_SIGNAL_UNCERTAINTY &&
                             adcVal <= (ushort) WindDirectionAdcResponse.NE + ADC_SIGNAL_UNCERTAINTY)
                    {
                        _windDirectionCounters[(ushort) WindDirection.NE]++;
                        _currentWindDirection = WindDirection.NE;
                    }

                    else if (adcVal >= (ushort) WindDirectionAdcResponse.E - ADC_SIGNAL_UNCERTAINTY &&
                             adcVal <= (ushort) WindDirectionAdcResponse.E + ADC_SIGNAL_UNCERTAINTY)
                    {
                        _windDirectionCounters[(ushort) WindDirection.E]++;
                        _currentWindDirection = WindDirection.E;
                    }

                    else if (adcVal >= (ushort) WindDirectionAdcResponse.SE - ADC_SIGNAL_UNCERTAINTY &&
                             adcVal <= (ushort) WindDirectionAdcResponse.SE + ADC_SIGNAL_UNCERTAINTY)
                    {
                        _windDirectionCounters[(ushort) WindDirection.SE]++;
                        _currentWindDirection = WindDirection.SE;
                    }

                    else if (adcVal >= (ushort) WindDirectionAdcResponse.S - ADC_SIGNAL_UNCERTAINTY &&
                             adcVal <= (ushort) WindDirectionAdcResponse.S + ADC_SIGNAL_UNCERTAINTY)
                    {
                        _windDirectionCounters[(ushort) WindDirection.S]++;
                        _currentWindDirection = WindDirection.S;
                    }

                    else if (adcVal >= (ushort) WindDirectionAdcResponse.SW - ADC_SIGNAL_UNCERTAINTY &&
                             adcVal <= (ushort) WindDirectionAdcResponse.SW + ADC_SIGNAL_UNCERTAINTY)
                    {
                        _windDirectionCounters[(ushort) WindDirection.SW]++;
                        _currentWindDirection = WindDirection.SW;
                    }

                    //esp32 ADC output is not linear - for voltages in range 3,2 - 3,3 the output value has same, maximum value = 4095. For wind direction "W" input from wind vane = 3,2V 
                    else if (adcVal >= (ushort) WindDirectionAdcResponse.W - 3 &&
                             adcVal <= (ushort) WindDirectionAdcResponse.W)
                    {
                        _windDirectionCounters[(int) WindDirection.W]++;
                        _currentWindDirection = WindDirection.W;
                    }

                    //because for wind direction NW input from wind vane = 3,10 V is near to maximum voltage which can be measured by ADC (3,2V) we must to use lower value uncertainty range
                    else if (adcVal >= (ushort) WindDirectionAdcResponse.NW - 60 &&
                             adcVal <= (ushort) WindDirectionAdcResponse.NW + 20)
                    {
                        _windDirectionCounters[(ushort) WindDirection.NW]++;
                        _currentWindDirection = WindDirection.NW;
                    }

                    else
                    {
                        _currentWindDirection = WindDirection.Undefined;
                    }

                    Logger.Log(() => $"Current wind vane direction: {_currentWindDirection } {adcVal}");


                    Thread.Sleep((ushort) measurementResolution * 1000);
                }
            });

            _measuringThread.Start();
        }

        public WindDirection GetCurrentWindDirection()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return _currentWindDirection;
        }

        public WindDirection GetMostFrequentWindDirection()
        {
            if (!_isMeasuring)
            {
                throw new InvalidOperationException("Cannot get data because measurement wasn't started");
            }

            return CalculateMostFrequentWindDirection();
        }

        public void Reset()
        {
            for (ushort i = 0; i < _windDirectionCounters.Length; i++)
            {
                _windDirectionCounters[i] = 0;
            }
        }

        private WindDirection CalculateMostFrequentWindDirection()
        {
            ushort maxCountValue = 0;
            ushort maxCountIndex = 0;

            for (ushort i = 0; i < _windDirectionCounters.Length; i++)
            {
                if (_windDirectionCounters[i] > maxCountValue)
                {
                    maxCountValue = _windDirectionCounters[i];
                    maxCountIndex = i;
                }
            }

            return (WindDirection) maxCountIndex;
        }

        public void StopMeasurement()
        {
            _isMeasuring = false;
            _measuringThread.Join();
            Reset();
        }
    }
}