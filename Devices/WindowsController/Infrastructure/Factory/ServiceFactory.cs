using System.Threading;
using WindowsController.Application.Services;
using WindowsController.Infrastructure.Config;
using EspIot.Application.Events.Outbound;
using EspIot.Application.Services;
using EspIot.Core.Helpers;
using EspIot.Core.Messaging.Enum;
using EspIot.Infrastructure.Handlers;
using EspIot.Infrastructure.MessageBus;
using EspIot.Infrastructure.Services;
using EspIot.Infrastructure.Wifi;
using nanoFramework.Runtime.Native;

namespace WindowsController.Infrastructure.Factory
{
    //TODO write Service register for manage services states and access to services instead of doing this in ServiceFactory  
    //TODO crate base service factory for shared services 
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

        private WindowsManagingService _windowsManagingService;

        public ServiceFactory(DriversFactory driversFactory, Configuration configuration)
        {
            _driversFactory = driversFactory;
            SetInitialPinStates();
            _configuration = configuration;
            _mqttOutboundEventBus = new MqttOutboundEventBus(_driversFactory.IotHubClient);
        }

        private ServiceFactory SetInitialPinStates()
        {
            _driversFactory.Window1Actuator.StopMoving();
            _driversFactory.Window2Actuator.StopMoving();

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
                new CommandHandlersFactory(_mqttOutboundEventBus, _diagnosticService, _windowsManagingService);
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

        public ServiceFactory InitWindowsManagingService()
        {
            //Reset actuator state

            _windowsManagingService = new WindowsManagingService(_driversFactory.Window1Actuator,
                _driversFactory.Window2Actuator, _driversFactory.Window1ReedSwitch, _driversFactory.Window2ReedSwitch,
                _driversFactory.Window1ControlSwitch, _driversFactory.Window2ControlSwitch,
                _mqttOutboundEventBus);

            _windowsManagingService.Start();

            Logger.Log(() => $"Free memory after init windows managing service {GC.Run(false)}");

            return this;
        }
    }
}