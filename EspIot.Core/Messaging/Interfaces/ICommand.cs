namespace EspIot.Core.Messaging.Interfaces
{
    public interface ICommand
    {
        string CorrelationId {get; }
    }
}
