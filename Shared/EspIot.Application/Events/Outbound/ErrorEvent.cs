using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Interfaces;

namespace EspIot.Application.Events.Outbound
{
    public class ErrorEvent : IEvent
    {
        public ErrorEvent(string message, ErrorLevel errorLevel)
        {
            Message = message;
            ErrorLevel = errorLevel;
        }

        public string Message { get;}
        public ErrorLevel ErrorLevel { get;}
    }
}
