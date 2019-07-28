using EspIot.Core.Messaging.Interfaces;

namespace Infrastructure.Commands.Command
{
    class SetConfigCommand : ICommand
    {
        public string CorrelationId { get; }
    }
}
