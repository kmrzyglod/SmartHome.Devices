using System;
using System.Threading;
using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;

namespace EspIot.Infrastructure.Services
{
    public class CommandDispatcherService
    {
        private readonly ICommandBus _commandBus;
        private readonly ICommandHandlersFactory _commandHandlersFactory;
        private readonly IOutboundEventBus _outboundEventBus;

        public CommandDispatcherService(ICommandHandlersFactory commandHandlersFactory, ICommandBus commandBus,
            IOutboundEventBus outboundEventBus)
        {
            _commandHandlersFactory = commandHandlersFactory;
            _commandBus = commandBus;
            _outboundEventBus = outboundEventBus;
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
                        string commandName = command.GetType().Name;
                        try
                        {
                            _commandHandlersFactory.Get(commandName).Handle(command);
                        }
                        catch (Exception e)
                        {
                            _outboundEventBus.Send(new CommandResultEvent(command.CorrelationId, StatusCode.Error,
                                commandName, e.Message));
                        }
                    }
                }).Start();
            };
        }
    }
}