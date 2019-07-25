using EspIot.Core.Messaging.Interfaces;

namespace Messages.Commands.Command
{
    class SetConfigCommand : ICommand
    {
        public string CorrelationId { get; }
    }
}
