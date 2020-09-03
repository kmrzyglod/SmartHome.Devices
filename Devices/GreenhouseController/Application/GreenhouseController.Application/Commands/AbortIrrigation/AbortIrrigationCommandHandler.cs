using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;
using EspIot.Core.Messaging.Enum;
using GreenhouseController.Application.Services.Irrigation;

namespace GreenhouseController.Application.Commands.AbortIrrigation
{
    public class AbortIrrigationCommandHandler: ICommandHandler
    {
        private readonly IrrigationService _irrigationService;
        private readonly IOutboundEventBus _outboundEventBus;

        public AbortIrrigationCommandHandler(IrrigationService irrigationService, IOutboundEventBus outboundEventBus)
        {
            _irrigationService = irrigationService;
            _outboundEventBus = outboundEventBus;
        }

        public void Handle(ICommand command)
        {
            Handle(command as AbortIrrigationCommand);
        }

        private void Handle(AbortIrrigationCommand command)
        {
            _irrigationService.FinishIrrigation();
            _outboundEventBus.Send(new CommandResultEvent(command.CorrelationId, StatusCode.Success, nameof(AbortIrrigationCommand)));
        }
    }
}
