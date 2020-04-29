using EspIot.Core.Messaging.Enum;

namespace EspIot.Application.Interfaces
{
    public interface ICommandResultEvent: IEvent, IMessage
    {
        string CommandName { get; }
        StatusCode Status { get;}
        string ErrorMessage { get;}
    }
}
