using System.Threading;
using EspIot.Core.Messaging.Interfaces;

namespace EspIot.Infrastructure.Services
{
    public class CommandDispatcherService
    {
        private readonly ICommandBus _commandBus;
        private readonly ICommandHandlersFactory _commandHandlersFactory;

        public CommandDispatcherService(ICommandHandlersFactory commandHandlersFactory, ICommandBus commandBus)
        {
            _commandHandlersFactory = commandHandlersFactory;
            _commandBus = commandBus;
        }

        public void Start()
        {
            _commandBus.OnNewCommandPartitionKeyCreated += (_, commandPartitionKey) =>
            {
                new Thread(() =>
                {
                    while (true)
                    {
                        var command = _commandBus.GetCommandQueue(commandPartitionKey).Dequeue() as ICommand;
                        _commandHandlersFactory.Get(nameof(command)).Handle(command);
                    }
                });
            };
        }
    }
}