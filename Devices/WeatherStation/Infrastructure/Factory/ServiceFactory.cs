using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Enum;
using EspIot.Infrastructure.Handlers;
using EspIot.Infrastructure.MessageBus;
using EspIot.Infrastructure.Services;
using EspIot.Infrastructure.Wifi;
using WeatherStation.Application.Services;
using WeatherStation.Infrastructure.Config;

namespace WeatherStation.Infrastructure.Factory
{
    public class ServiceFactory
    {
        private readonly WeatherStationConfiguration _configuration;
        private readonly DriversFactory _driversFactory;
        private readonly MqttOutboundEventBus _mqttOutboundEventBus;
        private CommandBus _commandBus;
        private CommandDispatcherService _commandDispatcherService;
        private CommandHandlersFactory _commandHandlersFactory;
        private CommandsFactory _commandsFactory;
        private InboundMessagesHandler _inboundMessagesHandler;
        private TelemetryService _telemetryService;

        public ServiceFactory(DriversFactory driversFactory, WeatherStationConfiguration configuration)
        {
            _driversFactory = driversFactory;
            SetInitialPinStates();
            _configuration = configuration;
            _mqttOutboundEventBus = new MqttOutboundEventBus(_driversFactory.IotHubClient);
        }

        private ServiceFactory SetInitialPinStates()
        {
            _driversFactory.StatusLed.SetWifiDisconnected();
            _driversFactory.StatusLed.SetMqttBrokerDisconnected();

            return this;
        }

        public ServiceFactory InitWifi()
        {
            WifiDriver.OnWifiConnected += () => { _driversFactory.StatusLed.SetWifiConnected(); };
            WifiDriver.OnWifiDisconnected += () => { _driversFactory.StatusLed.SetWifiDisconnected(); };
            WifiDriver.OnWifiDuringConnection += () => { _driversFactory.StatusLed.SetWifiDuringConnection(); };
            WifiDriver.ConnectToNetwork();

            return this;
        }

        public ServiceFactory InitMqttClient()
        {
            _driversFactory.IotHubClient.OnMqttClientConnected += () =>
            {
                _driversFactory.StatusLed.SetMqttBrokerConnected();
            };
            _driversFactory.IotHubClient.OnMqttClientDisconnected += () =>
            {
                _driversFactory.StatusLed.SetMqttBrokerDisconnected();
            };
            _driversFactory.IotHubClient.Connect();
            _driversFactory.IotHubClient.Subscribe(new[] {"devices/esp32-greenhouse/messages/devicebound/#"});
            _mqttOutboundEventBus.Send(new DeviceStatusUpdatedEvent("Device was turned on and connected to MQTT broker", DeviceStatusCode.DeviceWasTurnedOn));

            return this;
        }

        public ServiceFactory InitInboundMessagesProcessing()
        {
            _commandBus = new CommandBus(_mqttOutboundEventBus);
            _commandsFactory = new CommandsFactory();
            _commandHandlersFactory = new CommandHandlersFactory(_mqttOutboundEventBus, _telemetryService);
            _inboundMessagesHandler =
                new InboundMessagesHandler(_driversFactory.IotHubClient, _commandBus, _commandsFactory, _mqttOutboundEventBus);
            _commandDispatcherService = new CommandDispatcherService(_commandHandlersFactory, _commandBus, _mqttOutboundEventBus);
            _commandDispatcherService.Start();

            return this;
        }

        public ServiceFactory InitTelemetryService()
        {
            _telemetryService = new TelemetryService(_mqttOutboundEventBus,
                _driversFactory.AnemometerDriver,
                _driversFactory.WindVaneDriver,
                _driversFactory.RainGaugeDriver,
                _driversFactory.Bme280,
                _driversFactory.LightSensor);

            _telemetryService.Start();

            return this;
        }
    }
}