using EspIot.Core.Messaging.Interfaces;

namespace Messages.Commands.Command
{
    class CloseWindowCommand : ICommand
    {
        public CloseWindowCommand(string correlationId, short[] windowIds)
        {
            CorrelationId = correlationId;
            WindowIds = windowIds;
        }

        public string CorrelationId { get; }
        public short[] WindowIds { get;}
    }
}
