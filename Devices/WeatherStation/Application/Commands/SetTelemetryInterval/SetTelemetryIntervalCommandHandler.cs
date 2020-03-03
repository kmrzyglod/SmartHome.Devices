using EspIot.Application.Events.Outbound;
using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;
using WeatherStation.Application.Services;

namespace WeatherStation.Application.Commands.SetTelemetryInterval
{
    public class SetTelemetryIntervalCommandHandler : ICommandHandler

    {
        private readonly TelemetryService _telemetryService;
        private readonly IOutboundEventBus _outboundEventBus;

        public SetTelemetryIntervalCommandHandler(TelemetryService telemetryService, IOutboundEventBus outboundEventBus)
        {
            _telemetryService = telemetryService;
            _outboundEventBus = outboundEventBus;
        }

        public void Handle(ICommand command)
        {
            Handle(command as SetTelemetryIntervalCommand);
        }

        private void Handle(SetTelemetryIntervalCommand command)
        {
            _telemetryService.SetInterval(command.Interval);
            _outboundEventBus.Send(new CommandResultEvent(command.CorrelationId, StatusCode.Success, nameof(SetTelemetryIntervalCommand)));
        }
    }
}