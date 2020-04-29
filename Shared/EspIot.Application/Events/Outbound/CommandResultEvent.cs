using EspIot.Application.Interfaces;
using EspIot.Core.Messaging.Enum;

namespace EspIot.Application.Events.Outbound
{
    public class CommandResultEvent : ICommandResultEvent
    {
        public CommandResultEvent(string correlationId, StatusCode status, string commandName, string errorMessage = "")
        {
            CorrelationId = correlationId;
            CommandName = commandName;
            Status = status;
            ErrorMessage = errorMessage;
        }

        public string CorrelationId { get;}
        
        public string CommandName { get;}

        public StatusCode Status { get; }

        public string ErrorMessage { get; }
    }
}
