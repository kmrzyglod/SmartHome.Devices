using EspIot.Infrastructure.Handlers;
using EspIot.Infrastructure.MessageBus;
using EspIot.Infrastructure.Services;
using EspIot.Infrastructure.Wifi;
using WeatherStation.Infrastructure.Config;

namespace WeatherStation.Infrastructure
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

        public ServiceFactory(DriversFactory driversFactory, WeatherStationConfiguration configuration)
        {
            _driversFactory = driversFactory;
            _configuration = configuration;
            _mqttOutboundEventBus = new MqttOutboundEventBus(_driversFactory.IotHubClient);
        }

        public ServiceFactory InitWifi()
        {
            WifiDriver.OnWifiConnected += () => { _driversFactory.StatusLed.SetWifiConnected(); };
            WifiDriver.OnWifiDisconnected += () => { _driversFactory.StatusLed.SetWifiDisconnected(); };
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

            return this;
        }

        public ServiceFactory InitInboundMessagesProcessing()
        {
            _commandBus = new CommandBus(_mqttOutboundEventBus);
            _commandsFactory = new CommandsFactory();
            _commandHandlersFactory = new CommandHandlersFactory();
            _inboundMessagesHandler =
                new InboundMessagesHandler(_driversFactory.IotHubClient, _commandBus, _commandsFactory);
            _commandDispatcherService = new CommandDispatcherService(_commandHandlersFactory, _commandBus);
            _commandDispatcherService.Start();

            return this;
        }
    }
}