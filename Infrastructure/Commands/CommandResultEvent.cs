using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Interfaces;
using System;

namespace Infrastructure.Commands
{
    public class CommandResultEvent : ICommandResultEvent
    {
        public CommandResultEvent(string correlationId, StatusCode status, string errorMessage = "")
        {
            CorrelationId = correlationId;
            Status = status;
            ErrorMessage = errorMessage;
        }

        public string CorrelationId { get;}

        public StatusCode Status { get; }

        public string ErrorMessage { get; }
    }
}
