using EspIot.Core.Messaging.Interfaces;

namespace Infrastructure.Commands.Command
{
    class SetAutoModeCommand : ICommand
    {
        public string CorrelationId { get; }
        
        public short MaxOptimalTemperature { get;}
        public short MinOptimalTemperature { get;}
       
        public short MaxOptimalHumidity { get;}
        public short MinOptimalHumidity { get;}
        
        public short UpperCriticalTemperature { get;}
        public short LowerCriticalTemperature { get;}

        public short UpperCriticalHumidity { get; }
        public short LowerCriticalHumidity { get; }
    }
}
