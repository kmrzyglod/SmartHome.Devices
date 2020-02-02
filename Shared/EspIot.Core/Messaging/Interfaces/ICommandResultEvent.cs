using EspIot.Core.Messaging.Enum;

namespace EspIot.Core.Messaging.Interfaces
{
    public interface ICommandResultEvent: IEvent
    {
        string CorrelationId { get; }
        StatusCode Status { get;}
        string ErrorMessage { get;}
    }
}
