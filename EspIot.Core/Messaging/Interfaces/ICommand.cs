using EspIot.Core.Messaging.Validation;

namespace EspIot.Core.Messaging.Interfaces
{
    public interface ICommand
    {
        string PartitionKey { get; }
        string CorrelationId {get; }
        ValidationError[] Validate();
    }
}
