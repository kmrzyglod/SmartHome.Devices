using EspIot.Core.Messaging.Concrete;

namespace Infrastructure.Commands.Command
{
    internal class SetAutoModeCommand : CommandBase
    {
        public short MaxOptimalTemperature { get; }
        public short MinOptimalTemperature { get; }

        public short MaxOptimalHumidity { get; }
        public short MinOptimalHumidity { get; }

        public short UpperCriticalTemperature { get; }
        public short LowerCriticalTemperature { get; }

        public short UpperCriticalHumidity { get; }
        public short LowerCriticalHumidity { get; }

        public SetAutoModeCommand(string correlationId,
            short maxOptimalTemperature,
            short minOptimalTemperature,
            short maxOptimalHumidity,
            short minOptimalHumidity,
            short upperCriticalTemperature,
            short lowerCriticalTemperature,
            short upperCriticalHumidity,
            short lowerCriticalHumidity) : base(
            correlationId)
        {
            MaxOptimalTemperature = maxOptimalTemperature;
            MinOptimalTemperature = minOptimalTemperature;
            MaxOptimalHumidity = maxOptimalHumidity;
            MinOptimalHumidity = minOptimalHumidity;
            UpperCriticalTemperature = upperCriticalTemperature;
            LowerCriticalTemperature = lowerCriticalTemperature;
            UpperCriticalHumidity = upperCriticalHumidity;
            LowerCriticalHumidity = lowerCriticalHumidity;
        }
    }
}