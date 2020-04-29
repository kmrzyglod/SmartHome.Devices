using System.Collections;
using EspIot.Application.Events;
using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;
using EspIot.Core.Collections;
using EspIot.Core.Messaging.Enum;

namespace EspIot.Infrastructure.MessageBus
{
    public class CommandBus : ICommandBus
    {
        private const short MAXIMUM_QUEUE_COUNT = 2;
        private static readonly Hashtable _commandQueues = new Hashtable();
        private readonly IOutboundEventBus _outboundEventBus;

        public CommandBus(IOutboundEventBus outboundEventBus)
        {
            _outboundEventBus = outboundEventBus;
        }

        public event NewCommandPartitionKeyCreatedEventHandler OnNewCommandPartitionKeyCreated;

        public ConcurrentQueue GetCommandQueue(string partitionKey)
        {
            return _commandQueues[partitionKey] as ConcurrentQueue;
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
                OnNewCommandPartitionKeyCreated?.Invoke(this, command.PartitionKey);
            }
        }

        private bool AddCommandToQueue(ICommand command)
        {
            if (!_commandQueues.Contains(command.PartitionKey))
            {
                return false;
            }

            var commandQueue = _commandQueues[command.PartitionKey] as ConcurrentQueue;
            lock (_commandQueues)
            {
                if (commandQueue.Count > MAXIMUM_QUEUE_COUNT)
                {
                    var commandName = command.GetType().Name;
                    _outboundEventBus.Send(new CommandResultEvent(commandName, StatusCode.Refused,
                        $"Command queue overflow for command {commandName}."));
                    return true;
                }

                commandQueue?.Enqueue(command);
            }

            return true;
        }
    }
}