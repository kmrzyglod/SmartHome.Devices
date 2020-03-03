using EspIot.Application.Events.Outbound;
using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;

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