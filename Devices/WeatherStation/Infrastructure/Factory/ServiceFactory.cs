using EspIot.Application.Events.Outbound;
using EspIot.Application.Services;
using EspIot.Core.Helpers;
using EspIot.Core.Messaging.Enum;
using EspIot.Infrastructure.Handlers;
using EspIot.Infrastructure.MessageBus;
using EspIot.Infrastructure.Services;
using EspIot.Infrastructure.Wifi;
using nanoFramework.Runtime.Native;
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
        private IDiagnosticService _diagnosticService;
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
            Logger.Log(() => $"Free memory after connected to wifi {Debug.GC(false)}");

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
            _driversFactory.IotHubClient.Connect(new[] {_configuration.InboundMessagesTopic});
            _mqttOutboundEventBus.Send(new DeviceStatusUpdatedEvent("Device was turned on and connected to MQTT broker",
                DeviceStatusCode.DeviceWasTurnedOn));

            Logger.Log(() => $"Free memory after connected to MQTT broker {Debug.GC(false)}");

            return this;
        }

        public ServiceFactory InitInboundMessagesProcessing()
        {
            _commandBus = new CommandBus(_mqttOutboundEventBus);
            _commandsFactory = new CommandsFactory();
            _commandHandlersFactory =
                new CommandHandlersFactory(_mqttOutboundEventBus, _telemetryService, _diagnosticService);
            _inboundMessagesHandler =
                new InboundMessagesHandler(_driversFactory.IotHubClient, _commandBus, _commandsFactory,
                    _mqttOutboundEventBus);
            _commandDispatcherService =
                new CommandDispatcherService(_commandHandlersFactory, _commandBus, _mqttOutboundEventBus);
            _commandDispatcherService.Start();

            Logger.Log(() => $"Free memory after init inbound message processing {Debug.GC(false)}");

            return this;
        }

        public ServiceFactory InitDiagnosticService()
        {
            _diagnosticService = new DiagnosticService(_mqttOutboundEventBus);
            _diagnosticService.Start();
            Logger.Log(() => $"Free memory after init diagnostic service {Debug.GC(false)}");
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

            Logger.Log(() => $"Free memory after init telemetry service {Debug.GC(false)}");

            return this;
        }
    }
}