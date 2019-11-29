using System.Collections;
using EspIot.Core.Collections;
using EspIot.Core.Messaging.Interfaces;

namespace Infrastructure.Commands
{
    public class CommandBus: ICommandBus
    {
        private static readonly Hashtable _commandQueues = new Hashtable();

        public void Send(ICommand command)
        {
            if (_commandQueues.Contains(command.PartitionKey))
            {
                (_commandQueues[command.PartitionKey] as ConcurrentQueue)?.Enqueue(command);
            }
            else
            {
                var newQueue = new ConcurrentQueue();
                newQueue.Enqueue(command);
                _commandQueues.Add(command.PartitionKey, newQueue);
            }
        }
    }
}
