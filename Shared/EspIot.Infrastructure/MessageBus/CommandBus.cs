using System.Collections;
using EspIot.Core.Collections;
using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;

namespace EspIot.Infrastructure.MessageBus
{
    public class CommandBus : ICommandBus
    {
        private const short MAXIMUM_QUEUE_COUNT = 5;
        private static readonly Hashtable _commandQueues = new Hashtable();
        private readonly IOutboundEventBus _outboundEventBus;

        public CommandBus(IOutboundEventBus outboundEventBus)
        {
            _outboundEventBus = outboundEventBus;
        }

        public void Send(ICommand command)
        {
            if (AddCommandToQueue(command))
            {
                return;
            }

            lock (_commandQueues)
            {
                if (AddCommandToQueue(command))
                {
                    return;
                }

                var newQueue = new ConcurrentQueue();
                newQueue.Enqueue(command);
                _commandQueues.Add(command.PartitionKey, newQueue);
            }
        }

        private bool AddCommandToQueue(ICommand command)
        {
            if (!_commandQueues.Contains(command.PartitionKey))
            {
                return false;
            }

            var commandQueue = _commandQueues[command.PartitionKey] as ConcurrentQueue;
            if (commandQueue.Count > MAXIMUM_QUEUE_COUNT)
            {
                _outboundEventBus.Send(new ProcessingFailureEvent(StatusCode.Refused,
                    $"Command queue overflow for command {nameof(ICommand)}"));
                return true;
            }

            commandQueue?.Enqueue(command);
            return true;
        }
    }
}