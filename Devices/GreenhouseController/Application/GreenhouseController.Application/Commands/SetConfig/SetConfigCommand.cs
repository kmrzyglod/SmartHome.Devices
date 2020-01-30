using EspIot.Core.Messaging.Concrete;

namespace GreenhouseController.Application.Commands.SetConfig
{
    internal class SetConfigCommand : CommandBase
    {
        public SetConfigCommand(string correlationId) : base(correlationId)
        { }

        public override string PartitionKey { get; } = default;
    }
}