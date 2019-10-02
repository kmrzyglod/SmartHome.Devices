using EspIot.Core.Messaging.Validation;

namespace EspIot.Core.Messaging.Interfaces
{
    public interface ICommand
    {
        string CorrelationId {get; }
        ValidationError[] Validate();
    }
}
