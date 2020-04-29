using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;
using EspIot.Core.Messaging.Enum;
using WeatherStation.Application.Services;

namespace WeatherStation.Application.Commands.StartTelemetryService
{
    public class StartTelemetryServiceCommandHandler : ICommandHandler
    {
        private readonly IOutboundEventBus _outboundEventBus;
        private readonly TelemetryService _telemetryService;

        public StartTelemetryServiceCommandHandler(TelemetryService telemetryService,
            IOutboundEventBus outboundEventBus)
        {
            _telemetryService = telemetryService;
            _outboundEventBus = outboundEventBus;
        }

        public void Handle(ICommand command)
        {
            if (_telemetryService.IsRunning())
            {
                _outboundEventBus.Send(new CommandResultEvent(command.CorrelationId, StatusCode.Refused,
                    nameof(StartTelemetryServiceCommand), "Telemetry service is already running"));
                return;
            }

            _telemetryService.Start();
            _outboundEventBus.Send(new CommandResultEvent(command.CorrelationId, StatusCode.Success,
                nameof(StartTelemetryServiceCommand)));
        }
    }
}