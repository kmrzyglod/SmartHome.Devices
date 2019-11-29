using EspIot.Core.Messaging.Concrete;

namespace Infrastructure.Commands.Command
{
    internal class SetConfigCommand : CommandBase
    {
        public SetConfigCommand(string correlationId) : base(correlationId)
        { }

        public override string PartitionKey { get; } = default;
    }
}