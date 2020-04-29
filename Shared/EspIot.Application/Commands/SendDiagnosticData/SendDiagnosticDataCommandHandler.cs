using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;
using EspIot.Application.Services;
using EspIot.Core.Messaging.Enum;

namespace EspIot.Application.Commands.SendDiagnosticData
{
    public class SendDiagnosticDataCommandHandler : ICommandHandler
    {
        private readonly IDiagnosticService _diagnosticService;
        private readonly IOutboundEventBus _outboundEventBus;

        public SendDiagnosticDataCommandHandler(IOutboundEventBus outboundEventBus,
            IDiagnosticService diagnosticService)
        {
            _outboundEventBus = outboundEventBus;
            _diagnosticService = diagnosticService;
        }

        public void Handle(ICommand command)
        {
            _outboundEventBus.Send(_diagnosticService.GetDiagnosticDataEvent());
            _outboundEventBus.Send(new CommandResultEvent(command.CorrelationId, StatusCode.Success,
                nameof(SendDiagnosticDataCommand)));
        }
    }
}