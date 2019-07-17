using EspIot.Core.Messaging.Interfaces;

namespace Messages.Commands.Command
{
    public class SetTelemetryIntervalCommand : ICommand
    {
        public string CorrelationId { get; }
        public int Interval { get; }
    }
}
