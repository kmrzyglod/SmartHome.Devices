using EspIot.Core.Messaging.Interfaces;

namespace Infrastructure.Commands.Command
{
    class SetManualModeCommand: ICommand
    {
        public string CorrelationId { get; }
    }
}
