using EspIot.Core.Messaging.Interfaces;

namespace Messages.Commands.Command
{
    class OpenWindowCommand: ICommand
    {
        public OpenWindowCommand(string correlationId, short[] windowIds)
        {
            CorrelationId = correlationId;
            WindowIds = windowIds;
        }

        public string CorrelationId { get; }
        public short[] WindowIds { get; }
    }
}
