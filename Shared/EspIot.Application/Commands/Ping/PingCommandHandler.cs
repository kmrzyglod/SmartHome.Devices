using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;
using EspIot.Core.Messaging.Enum;

namespace EspIot.Application.Commands.Ping
{
    public class PingCommandHandler : ICommandHandler
    {
        private readonly IOutboundEventBus _outboundEventBus;

        public PingCommandHandler(IOutboundEventBus outboundEventBus)
        {
            _outboundEventBus = outboundEventBus;
        }

        public void Handle(ICommand command)
        {
            _outboundEventBus.Send(new CommandResultEvent(command.CorrelationId, StatusCode.Success, nameof(PingCommand)));
        }
    }
}