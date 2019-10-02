using EspIot.Core.Messaging.Concrete;

namespace Infrastructure.Commands.Command
{
    internal class SetManualModeCommand : CommandBase
    {
        public SetManualModeCommand(string correlationId) : base(correlationId)
        { }
    }
}