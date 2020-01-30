using EspIot.Core.Messaging.Concrete;
using GreenhouseController.Application.Services.AutomaticControl;

namespace GreenhouseController.Application.Commands.SetManualMode
{
    internal class SetManualModeCommand : CommandBase
    {
        public SetManualModeCommand(string correlationId) : base(correlationId)
        { }

        public override string PartitionKey { get; } = nameof(AutomaticControlService);
    }
}