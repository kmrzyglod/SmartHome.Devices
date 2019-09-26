using EspIot.Core.Messaging.Interfaces;

namespace Infrastructure.Commands.Command
{
    class SetTelemetryIntervalCommand : ICommand
    {
        public string CorrelationId { get; }
        //Telemetry send interval in seconds
        public int Interval { get;}
    }
}
