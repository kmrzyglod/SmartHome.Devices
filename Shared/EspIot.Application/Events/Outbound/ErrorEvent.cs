using EspIot.Application.Interfaces;
using EspIot.Core.Messaging.Enum;

namespace EspIot.Application.Events.Outbound
{
    public class ErrorEvent : EventBase
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
