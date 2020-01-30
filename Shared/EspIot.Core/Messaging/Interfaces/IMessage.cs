namespace EspIot.Core.Messaging.Interfaces
{
    public interface IMessage
    {
        string CorrelationId { get; }
    }
}
