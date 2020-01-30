using EspIot.Core.Messaging.Enum;

namespace EspIot.Core.Messaging.Interfaces
{
    public interface ICommandResultEvent
    {
        string CorrelationId { get; }
        StatusCode Status { get;}
        string ErrorMessage { get;}
    }
}
