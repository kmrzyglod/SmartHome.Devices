using EspIot.Infrastructure.Mappers;
using EspIot.Infrastructure.MessageBus;
using EspIot.Infrastructure.Wifi;
using GreenhouseController.Application.Events.Outbound;
using GreenhouseController.Application.Services.Telemetry;
using GreenhouseController.Application.Services.WindowsManager;
using Infrastructure.Config;

namespace Infrastructure.Factory
{
    public class ServiceFactory
    {
        private readonly DriversFactory _driversFactory;
        private readonly GreenhouseControllerConfiguration _configuration;
        private readonly MqttOutboundEventBus _mqttOutboundEventBus;
        private readonly CommandBus _commandBus;
        private readonly CommandsFactory _commandsFactory;

        private TelemetryService _telemetryService;
        private WindowsManagerService _windowsManagerService;

        public ServiceFactory(DriversFactory driversFactory, GreenhouseControllerConfiguration configuration)
        {
            _driversFactory = driversFactory;
            _configuration = configuration;
            _mqttOutboundEventBus = new MqttOutboundEventBus(_driversFactory.IotHubClient);
            _commandBus = new CommandBus(_mqttOutboundEventBus);
            _commandsFactory = new CommandsFactory();
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
            _driversFactory.IotHubClient.OnMqttClientConnected += () => { _driversFactory.StatusLed.SetMqttBrokerConnected(); };
            _driversFactory.IotHubClient.OnMqttClientDisconnected += () => { _driversFactory.StatusLed.SetMqttBrokerDisconnected(); };
            _driversFactory.IotHubClient.Connect();
            _driversFactory.IotHubClient.Subscribe(new[] { "devices/esp32-greenhouse/messages/devicebound/#" });

            return this;
        }

        public ServiceFactory InitInboundMessagesProcessing()
        {
            var commandsFactory = new InboundMessagesMapper(_driversFactory.IotHubClient, _commandBus, _commandsFactory);
            return this;
        }
        
        public ServiceFactory InitTelemetry()
        {
            _telemetryService = new TelemetryService(
                _driversFactory.DoorReedSwitch,
                _driversFactory.Window1ReedSwitch,
                _driversFactory.Window2ReedSwitch,
                _mqttOutboundEventBus,
                _driversFactory.LightSensor,
                _driversFactory.Bme280,
                _driversFactory.SoilMoistureSensor,
                _driversFactory.WaterFlowSensor
                );

            _telemetryService.SetInterval(10000);
            _telemetryService.Start();

            return this;
        }

        public ServiceFactory InitWindowsManager()
        {
            _windowsManagerService = new WindowsManagerService(
                _driversFactory.Window1Actuator,
                _driversFactory.Window2Actuator,
                _driversFactory.Window1ReedSwitch,
                _driversFactory.Window2ReedSwitch,
                _driversFactory.DoorReedSwitch,
                _mqttOutboundEventBus
                );

            return this;
        }

    }
}
