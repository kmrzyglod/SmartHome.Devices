using EspIot.Core.Messaging.Interfaces;

namespace Infrastructure.Commands.Command
{
    class CloseWindowCommand : ICommand
    {
        public CloseWindowCommand(string correlationId, ushort[] windowIds)
        {
            CorrelationId = correlationId;
            WindowIds = windowIds;
        }

        public string CorrelationId { get; }
        public ushort[] WindowIds { get;}
    }
}
