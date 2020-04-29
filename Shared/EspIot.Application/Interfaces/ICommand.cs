using EspIot.Core.Messaging.Validation;

namespace EspIot.Application.Interfaces
{
    public interface ICommand
    {
        string PartitionKey { get; }
        string CorrelationId {get; }
        ValidationError[] Validate();
    }
}
