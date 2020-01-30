
namespace EspIot.Core.Messaging.Interfaces
{
    public interface ICommandHandler
    {
        void Handle(object command);
    }
}
