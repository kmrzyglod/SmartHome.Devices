using EspIot.Core.Messaging.Interfaces;

namespace Messages.Commands.Command
{
    class IrrigateCommand : ICommand
    {
        public string CorrelationId { get;}
        public int IrrigationTime { get;}
    }
}
