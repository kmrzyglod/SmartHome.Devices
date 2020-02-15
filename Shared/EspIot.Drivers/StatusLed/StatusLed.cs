using System.Threading;
using Windows.Devices.Gpio;
using EspIot.Core.Gpio;

namespace EspIot.Drivers.StatusLed
{
    public class StatusLed
    {
        private readonly GpioPins _connectionStatus;
        private readonly GpioController _gpioController;
        private readonly GpioPin _mqttStatusLed;
        private readonly GpioPin _wifiStatusLed;


        private bool _isWifiLedBlinking;
        private Thread _wifiLedBlinkingThread = new Thread(() => { });

        public StatusLed(GpioController gpioController, GpioPins wifiStatus, GpioPins mqttStatus)
        {
            _gpioController = gpioController;
            _connectionStatus = mqttStatus;
            _wifiStatusLed = _gpioController.OpenPin((int) wifiStatus);
            _mqttStatusLed = _gpioController.OpenPin((int) mqttStatus);
            _wifiStatusLed.SetDriveMode(GpioPinDriveMode.Output);
            _mqttStatusLed.SetDriveMode(GpioPinDriveMode.Output);
            _wifiStatusLed.Write(GpioPinValue.Low);
            _mqttStatusLed.Write(GpioPinValue.Low);
        }

        public void SetWifiDuringConnection()
        {
            if (_wifiLedBlinkingThread.IsAlive)
            {
                return;
            }

            _wifiLedBlinkingThread = new Thread(() =>
            {
                _isWifiLedBlinking = true;
                while (_isWifiLedBlinking)
                {
                    _wifiStatusLed.Write(GpioPinValue.High);
                    Thread.Sleep(200);
                    _wifiStatusLed.Write(GpioPinValue.Low);
                    Thread.Sleep(200);
                }
            });

            _wifiLedBlinkingThread.Start();
        }

        public void SetWifiConnected()
        {
            new Thread(() =>
                {
                    _isWifiLedBlinking = false;
                    if (_wifiLedBlinkingThread.IsAlive)
                    {
                        _wifiLedBlinkingThread.Join();
                    }

                    _wifiStatusLed.Write(GpioPinValue.High);
                })
                .Start();
        }

        public void SetWifiDisconnected()
        {
            new Thread(() =>
                {
                    _isWifiLedBlinking = false;
                    if (_wifiLedBlinkingThread.IsAlive)
                    {
                        _wifiLedBlinkingThread.Join();
                    }

                    _wifiStatusLed.Write(GpioPinValue.Low);
                })
                .Start();
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