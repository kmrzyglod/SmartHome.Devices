using EspIot.Application.Events;
using EspIot.Core.Collections;

namespace EspIot.Application.Interfaces
{
    public interface ICommandBus
    {
        event NewCommandPartitionKeyCreatedEventHandler OnNewCommandPartitionKeyCreated;
        ConcurrentQueue GetCommandQueue(string partitionKey);
        void Send(ICommand command);
    }
}