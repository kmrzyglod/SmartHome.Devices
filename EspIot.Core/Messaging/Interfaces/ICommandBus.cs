namespace EspIot.Core.Messaging.Interfaces
{
    public interface ICommandBus
    {
        void Send(ICommand command);
    }
}
