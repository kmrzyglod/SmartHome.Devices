using EspIot.Core.Messaging.Enum;

namespace Services.Events
{
    public class FailureEvent
    {
        public FailureEvent(StatusCode status, string errorMessage)
        {
            Status = status;
            ErrorMessage = errorMessage;
        }

        public StatusCode Status { get; }
        public string ErrorMessage { get; }
    }
}
