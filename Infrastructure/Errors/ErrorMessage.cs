using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Interfaces;

namespace Infrastructure.Errors
{
    public class ErrorMessage : IMessage
    {
        public string CorrelationId { get;}
        public string Message { get;}
        public ErrorLevel ErrorLevel { get;}
    }
}
