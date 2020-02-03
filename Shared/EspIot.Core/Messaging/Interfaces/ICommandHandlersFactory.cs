namespace EspIot.Core.Messaging.Interfaces
{
    public interface ICommandHandlersFactory
    {
        ICommandHandler Get(string commandName);
    }
}
