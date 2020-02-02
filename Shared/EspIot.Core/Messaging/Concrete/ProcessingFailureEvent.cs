using System;
using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Interfaces;

namespace EspIot.Core.Messaging.Concrete
{
    public class ProcessingFailureEvent: IEvent
    {
        public ProcessingFailureEvent(StatusCode status, string errorMessage)
        {
            Status = status;
            ErrorMessage = errorMessage;
            CorrelationId = Guid.NewGuid().ToString();
        }

        public StatusCode Status { get; }
        public string ErrorMessage { get; }
        public string CorrelationId { get; }
    }
}
