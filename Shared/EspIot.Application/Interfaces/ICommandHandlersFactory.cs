namespace EspIot.Application.Interfaces
{
    public interface ICommandHandlersFactory
    {
        ICommandHandler Get(string commandName);
    }
}
