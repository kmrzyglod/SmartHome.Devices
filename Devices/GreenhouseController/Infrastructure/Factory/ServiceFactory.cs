using System.Threading;
using EspIot.Application.Events.Outbound;
using EspIot.Application.Services;
using EspIot.Core.Helpers;
using EspIot.Core.Messaging.Enum;
using EspIot.Infrastructure.Handlers;
using EspIot.Infrastructure.MessageBus;
using EspIot.Infrastructure.Services;
using EspIot.Infrastructure.Wifi;
using GreenhouseController.Application.Services.Door;
using GreenhouseController.Application.Services.EnvironmentalConditions;
using GreenhouseController.Application.Services.Irrigation;
using GreenhouseController.Application.Services.Telemetry;
using Infrastructure.Config;
using nanoFramework.Runtime.Native;

namespace Infrastructure.Factory
{
    public class ServiceFactory
    {
        private readonly Configuration _configuration;
        private readonly DriversFactory _driversFactory;
        private readonly MqttOutboundEventBus _mqttOutboundEventBus;
        private CommandBus _commandBus;
        private CommandDispatcherService _commandDispatcherService;
        private CommandHandlersFactory _commandHandlersFactory;
        private CommandsFactory _commandsFactory;
        private IDiagnosticService _diagnosticService;
        private InboundMessagesHandler _inboundMessagesHandler;
        private IrrigationService _irrigationService;
        private DoorService _doorService;
        private EnvironmentalConditionsService _environmentalConditionsService;

        private TelemetryService _telemetryService;

        public ServiceFactory(DriversFactory driversFactory, Configuration configuration)
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
            new Thread(() =>
                {
                    WifiDriver.ConnectToNetwork();
                    Logger.Log(() => $"Free memory after connected to wifi {GC.Run(false)}");
                })
                .Start();

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
            new Thread(() =>
            {
                _driversFactory.IotHubClient.Connect(new[] {_configuration.InboundMessagesTopic});
                _mqttOutboundEventBus.Send(new DeviceStatusUpdatedEvent(
                    "Device was turned on and connected to MQTT broker",
                    DeviceStatusCode.DeviceWasTurnedOn));

                Logger.Log(() => $"Free memory after connected to MQTT broker {GC.Run(false)}");
            }).Start();


            return this;
        }

        public ServiceFactory InitInboundMessagesProcessing()
        {
            _commandBus = new CommandBus(_mqttOutboundEventBus);
            _commandsFactory = new CommandsFactory();
            _commandHandlersFactory =
                new CommandHandlersFactory(_mqttOutboundEventBus, _diagnosticService, _irrigationService);
            _inboundMessagesHandler =
                new InboundMessagesHandler(_driversFactory.IotHubClient, _commandBus, _commandsFactory,
                    _mqttOutboundEventBus);
            _commandDispatcherService =
                new CommandDispatcherService(_commandHandlersFactory, _commandBus, _mqttOutboundEventBus);
            _commandDispatcherService.Start();

            Logger.Log(() => $"Free memory after init inbound message processing {GC.Run(false)}");

            return this;
        }

        public ServiceFactory InitDiagnosticService()
        {
            _diagnosticService = new DiagnosticService(_mqttOutboundEventBus);
            _diagnosticService.Start();
            Logger.Log(() => $"Free memory after init diagnostic service {GC.Run(false)}");
            return this;
        }


        public ServiceFactory InitTelemetry()
        {
            _telemetryService = new TelemetryService(
                _driversFactory.DoorReedSwitch,
                _mqttOutboundEventBus,
                _driversFactory.LightSensor,
                _driversFactory.Bme280,
                _driversFactory.SoilMoistureSensor
            );

            _telemetryService.Start();

            return this;
        }

        public ServiceFactory InitIrrigationService()
        {
            _irrigationService = new IrrigationService(_driversFactory.SolidStateRelaysDriver, _configuration.WaterPumpRelaySwitchChannel, _configuration.ValveRelaySwitchChannel, _driversFactory.WaterFlowSensorDriver, _mqttOutboundEventBus);
            _irrigationService.Start();
            Logger.Log(() => $"Free memory after init irrigation service {GC.Run(false)}");
            return this;
        }

        public ServiceFactory InitEnvironmentalConditionsService()
        {
            _environmentalConditionsService = new EnvironmentalConditionsService();
            Logger.Log(() => $"Free memory after init environmental condition service {GC.Run(false)}");
            return this;
        }

        public ServiceFactory InitDoorService()
        {
            _doorService = new DoorService(_mqttOutboundEventBus, _driversFactory.DoorReedSwitch);
            _doorService.Start();
            Logger.Log(() => $"Free memory after init door service {GC.Run(false)}");
            return this;
        }
    }
}