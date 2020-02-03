using EspIot.Core.Collections;
using EspIot.Core.Messaging.Events;

namespace EspIot.Core.Messaging.Interfaces
{
    public interface ICommandBus
    {
        event NewCommandPartitionKeyCreatedEventHandler OnNewCommandPartitionKeyCreated;
        ConcurrentQueue GetCommandQueue(string partitionKey);
        void Send(ICommand command);
    }
}