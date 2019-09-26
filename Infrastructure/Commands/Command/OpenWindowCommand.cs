using EspIot.Core.Messaging.Interfaces;

namespace Infrastructure.Commands.Command
{
    class OpenWindowCommand: ICommand
    {
        public OpenWindowCommand(string correlationId, ushort[] windowIds)
        {
            CorrelationId = correlationId;
            WindowIds = windowIds;
        }

        public string CorrelationId { get; }
        public ushort[] WindowIds { get; }
    }
}
