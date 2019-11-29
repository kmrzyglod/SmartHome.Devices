using EspIot.Core.Messaging.Concrete;
using Infrastructure.Services.AutomaticControl;

namespace Infrastructure.Commands.Command
{
    internal class SetManualModeCommand : CommandBase
    {
        public SetManualModeCommand(string correlationId) : base(correlationId)
        { }

        public override string PartitionKey { get; } = nameof(AutomaticControlService);
    }
}