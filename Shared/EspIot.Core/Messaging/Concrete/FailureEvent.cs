using EspIot.Core.Messaging.Enum;

namespace EspIot.Core.Messaging.Concrete
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
