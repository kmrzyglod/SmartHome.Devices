using Windows.Devices.Gpio;
using WindowsController.Infrastructure.Config;
using EspIot.Drivers.LinearActuator;
using EspIot.Drivers.StatusLed;
using EspIot.Drivers.Switch;
using EspIot.Infrastructure.Mqtt;

namespace WindowsController.Infrastructure.Factory
{
    public class DriversFactory
    {
        private readonly Configuration _configuration;
        private MqttClientWrapper _iotHubClient;

        private StatusLed _statusLed;
        private LinearActuatorDriver _window1Actuator;
        private SwitchDriver _window1ReedSwitch;
        private LinearActuatorDriver _window2Actuator;
        private SwitchDriver _window2ReedSwitch;
        private SwitchDriver _window1ControlSwitch;
        private SwitchDriver _window2ControlSwitch;

        public DriversFactory(Configuration configuration)
        {
            _configuration = configuration;
        }

        public StatusLed StatusLed => _statusLed ?? (_statusLed = new StatusLed(GpioController.GetDefault(),
            _configuration.WifiStatusLed, _configuration.MqttStatusLed));

        public MqttClientWrapper IotHubClient => _iotHubClient ?? (_iotHubClient =
            new MqttClientWrapper(_configuration.MqttBrokerAddress,
                _configuration.DeviceId, _statusLed));

        public LinearActuatorDriver Window1Actuator => _window1Actuator ?? (_window1Actuator =
            new LinearActuatorDriver(GpioController.GetDefault(),
                _configuration.FirstChannelInput1,
                _configuration.FirstChannelInput2,
                _configuration.FirstChannelEnable));

        public LinearActuatorDriver Window2Actuator => _window2Actuator ?? (_window2Actuator =
            new LinearActuatorDriver(GpioController.GetDefault(),
                _configuration.SecondChannelInput1,
                _configuration.SecondChannelInput2,
                _configuration.SecondChannelEnable));

        public SwitchDriver Window1ReedSwitch => _window1ReedSwitch ?? (_window1ReedSwitch =
            new SwitchDriver(GpioController.GetDefault(),
                _configuration.FirstChannelReedSwitch, GpioPinDriveMode.InputPullUp));

        public SwitchDriver Window2ReedSwitch => _window2ReedSwitch ?? (_window2ReedSwitch =
            new SwitchDriver(GpioController.GetDefault(),
                _configuration.SecondChannelReedSwitch, GpioPinDriveMode.InputPullUp));

        public SwitchDriver Window1ControlSwitch => _window1ControlSwitch ?? (_window1ControlSwitch =
            new SwitchDriver(GpioController.GetDefault(),
                _configuration.FirstChannelControlSwitch, GpioPinDriveMode.InputPullUp));

        public SwitchDriver Window2ControlSwitch => _window2ControlSwitch ?? (_window2ControlSwitch =
            new SwitchDriver(GpioController.GetDefault(),
                _configuration.SecondChannelControlSwitch, GpioPinDriveMode.InputPullUp));
    }
}