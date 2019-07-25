using EspIot.Core.Messaging.Interfaces;

namespace Messages.Commands.Command
{
    class SetManualModeCommand: ICommand
    {
        public string CorrelationId { get; }
    }
}
