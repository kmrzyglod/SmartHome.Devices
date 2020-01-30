using EspIot.Core.Gpio;
using Windows.Devices.Gpio;

namespace EspIot.Drivers.StatusLed
{
    public class StatusLed
    {
        private readonly GpioController _gpioController;
        private readonly GpioPins _connectionStatus;
        private readonly GpioPin _wifiStatusLed;
        private readonly GpioPin _mqttStatusLed;

        public StatusLed(GpioController gpioController, GpioPins wifiStatus, GpioPins mqttStatus)
        {
            _gpioController = gpioController;
            _connectionStatus = mqttStatus;
            _wifiStatusLed = _gpioController.OpenPin((int)wifiStatus);
            _mqttStatusLed = _gpioController.OpenPin((int)mqttStatus);
            _wifiStatusLed.SetDriveMode(GpioPinDriveMode.Output);
            _mqttStatusLed.SetDriveMode(GpioPinDriveMode.Output);
            _wifiStatusLed.Write(GpioPinValue.Low);
            _mqttStatusLed.Write(GpioPinValue.Low);
        }

        public void SetWifiConnected()
        {
            _wifiStatusLed.Write(GpioPinValue.High);
        }

        public void SetWifiDisconnected()
        {
            _wifiStatusLed.Write(GpioPinValue.Low);
        }

        public void SetMqttBrokerDisconnected()
        {
            _mqttStatusLed.Write(GpioPinValue.Low);
        }

        public void SetMqttBrokerConnected()
        {
            _mqttStatusLed.Write(GpioPinValue.High);

        }

    }
}
