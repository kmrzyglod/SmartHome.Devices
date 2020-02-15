using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Interfaces;

namespace EspIot.Core.Messaging.Concrete
{
    public class ErrorEvent : IEvent
    {
        public ErrorEvent(string correlationId, string message, ErrorLevel errorLevel)
        {
            CorrelationId = correlationId;
            Message = message;
            ErrorLevel = errorLevel;
        }

        public string CorrelationId { get;}
        public string Message { get;}
        public ErrorLevel ErrorLevel { get;}
    }
}
