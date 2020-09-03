using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;
using EspIot.Core.Messaging.Enum;
using GreenhouseController.Application.Services.Irrigation;

namespace GreenhouseController.Application.Commands.Irrigate
{
    public class IrrigateCommandHandler: ICommandHandler
    {
        private readonly IrrigationService _irrigationService;
        private readonly IOutboundEventBus _outboundEventBus;
        
        public IrrigateCommandHandler(IrrigationService irrigationService, IOutboundEventBus outboundEventBus)
        {
            _irrigationService = irrigationService;
            _outboundEventBus = outboundEventBus;
        }

        public void Handle(ICommand command)
        {
            Handle(command as IrrigateCommand);
        }

        private void Handle(IrrigateCommand command)
        {
            _irrigationService.StartIrrigation(command.MaximumIrrigationTime, command.WaterVolume);
            _outboundEventBus.Send(new CommandResultEvent(command.CorrelationId, StatusCode.Success, nameof(IrrigateCommand)));
        }
    }
}
