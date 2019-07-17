using EspIot.Core.Messaging.Interfaces;

namespace Messages.Commands.Command
{
    public class SetAutoModeConfigCommand: ICommand
    {
        public string CorrelationId { get; }
    }
}
