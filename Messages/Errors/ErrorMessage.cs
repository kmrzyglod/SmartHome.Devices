using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Interfaces;

namespace Messages.Errors.Events
{
    public class ErrorMessage : IMessage
    {
        public string CorrelationId { get;}
        public string Message { get;}
        public ErrorLevel ErrorLevel { get;}
    }
}
