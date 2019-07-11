using EspIot.Core.Messaging.Interfaces;

namespace Messages.Commands.Command
{
    public class ScheduleIrrigationCommand : ICommand
    {
        public string CorrelationId { get;}
    }
}
