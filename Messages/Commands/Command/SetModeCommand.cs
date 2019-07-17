using EspIot.Core.Messaging.Interfaces;

namespace Messages.Commands.Command
{
    public class SetModeCommand: ICommand
    {
        public string CorrelationId { get; }
    }
}
