using EspIot.Core.Messaging.Interfaces;

namespace Messages.Commands.Command
{
    public class CloseWindowCommand : ICommand
    {
        public string CorrelationId { get; }
        public ushort WindowNum { get; }
    }
}
