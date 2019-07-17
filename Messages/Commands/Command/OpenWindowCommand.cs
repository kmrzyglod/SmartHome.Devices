using EspIot.Core.Messaging.Interfaces;

namespace Messages.Commands.Command
{
    class OpenWindowCommand: ICommand
    {
        public string CorrelationId { get; }
        public ushort WindowNum { get; }
    }
}
