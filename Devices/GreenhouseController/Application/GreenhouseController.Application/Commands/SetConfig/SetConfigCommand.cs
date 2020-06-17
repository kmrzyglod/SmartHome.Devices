using System.Collections;
using EspIot.Application.Commands;
using EspIot.Application.Interfaces;
using EspIot.Core.Extensions;
using EspIot.Core.Messaging.Validation;

namespace GreenhouseController.Application.Commands.SetConfig
{
    internal class SetConfigCommand : CommandBase
    {
        public SetConfigCommand(string correlationId, short dayVegetationMaxOptimalTemperature,
            short dayVegetationMinOptimalTemperature, short nightVegetationMaxOptimalTemperature,
            short nightVegetationMinOptimalTemperature, short maxOptimalHumidity, short minOptimalHumidity,
            short upperCriticalTemperature, short lowerCriticalTemperature, short upperCriticalHumidity,
            short lowerCriticalHumidity, short upperCriticalSoilMoisture, short lowerCriticalSoilMoisture) : base(
            correlationId)
        {
            DayVegetationMaxOptimalTemperature = dayVegetationMaxOptimalTemperature;
            DayVegetationMinOptimalTemperature = dayVegetationMinOptimalTemperature;
            NightVegetationMaxOptimalTemperature = nightVegetationMaxOptimalTemperature;
            NightVegetationMinOptimalTemperature = nightVegetationMinOptimalTemperature;
            MaxOptimalHumidity = maxOptimalHumidity;
            MinOptimalHumidity = minOptimalHumidity;
            UpperCriticalTemperature = upperCriticalTemperature;
            LowerCriticalTemperature = lowerCriticalTemperature;
            UpperCriticalHumidity = upperCriticalHumidity;
            LowerCriticalHumidity = lowerCriticalHumidity;
            UpperCriticalSoilMoisture = upperCriticalSoilMoisture;
            LowerCriticalSoilMoisture = lowerCriticalSoilMoisture;
        }

        public short DayVegetationMaxOptimalTemperature { get; }
        public short DayVegetationMinOptimalTemperature { get; }

        public short NightVegetationMaxOptimalTemperature { get; }
        public short NightVegetationMinOptimalTemperature { get; }

        public short MaxOptimalHumidity { get; }
        public short MinOptimalHumidity { get; }

        public short UpperCriticalTemperature { get; }
        public short LowerCriticalTemperature { get; }

        public short UpperCriticalHumidity { get; }
        public short LowerCriticalHumidity { get; }

        public short UpperCriticalSoilMoisture { get; }
        public short LowerCriticalSoilMoisture { get; }

        public override string PartitionKey { get; } = default;

        public override ValidationError[] Validate()
        {
            var errors = ValidateBase();
            if (DayVegetationMaxOptimalTemperature <= DayVegetationMinOptimalTemperature)
            {
                errors.Add(new ValidationError(nameof(DayVegetationMaxOptimalTemperature),
                    $"{nameof(DayVegetationMaxOptimalTemperature)} field cannot has value lower or equal to field {nameof(DayVegetationMinOptimalTemperature)}"));
            }

            if (DayVegetationMinOptimalTemperature >= DayVegetationMaxOptimalTemperature)
            {
                errors.Add(new ValidationError(nameof(DayVegetationMinOptimalTemperature),
                    $"{nameof(DayVegetationMinOptimalTemperature)} field cannot has value higher or equal to field {nameof(DayVegetationMaxOptimalTemperature)}"));
            }

            if (NightVegetationMaxOptimalTemperature <= NightVegetationMinOptimalTemperature)
            {
                errors.Add(new ValidationError(nameof(NightVegetationMaxOptimalTemperature),
                    $"{nameof(NightVegetationMaxOptimalTemperature)} field cannot has value lower or equal to field {nameof(NightVegetationMinOptimalTemperature)}"));
            }

            if (NightVegetationMinOptimalTemperature >= NightVegetationMaxOptimalTemperature)
            {
                errors.Add(new ValidationError(nameof(NightVegetationMinOptimalTemperature),
                    $"{nameof(NightVegetationMinOptimalTemperature)} field cannot has value higher or equal to field {nameof(NightVegetationMaxOptimalTemperature)}"));
            }

            if (MaxOptimalHumidity <= MinOptimalHumidity)
            {
                errors.Add(new ValidationError(nameof(MaxOptimalHumidity),
                    $"{nameof(MaxOptimalHumidity)} field cannot has value lower or equal to field {nameof(MinOptimalHumidity)}"));
            }

            if (MinOptimalHumidity >= MaxOptimalHumidity)
            {
                errors.Add(new ValidationError(nameof(MinOptimalHumidity),
                    $"{nameof(MinOptimalHumidity)} field cannot has value higher or equal to field {nameof(MaxOptimalHumidity)}"));
            }

            if (UpperCriticalTemperature <= LowerCriticalTemperature)
            {
                errors.Add(new ValidationError(nameof(UpperCriticalTemperature),
                    $"{nameof(UpperCriticalTemperature)} field cannot has value lower or equal to field {nameof(LowerCriticalTemperature)}"));
            }

            if (LowerCriticalTemperature >= UpperCriticalTemperature)
            {
                errors.Add(new ValidationError(nameof(LowerCriticalTemperature),
                    $"{nameof(LowerCriticalTemperature)} field cannot has value higher or equal to field {nameof(UpperCriticalTemperature)}"));
            }

            if (UpperCriticalHumidity <= LowerCriticalHumidity)
            {
                errors.Add(new ValidationError(nameof(UpperCriticalHumidity),
                    $"{nameof(UpperCriticalHumidity)} field cannot has value lower or equal to field {nameof(LowerCriticalHumidity)}"));
            }

            if (LowerCriticalHumidity >= UpperCriticalHumidity)
            {
                errors.Add(new ValidationError(nameof(LowerCriticalHumidity),
                    $"{nameof(LowerCriticalHumidity)} field cannot has value higher or equal to field {nameof(UpperCriticalHumidity)}"));
            }

            if (UpperCriticalSoilMoisture <= LowerCriticalSoilMoisture)
            {
                errors.Add(new ValidationError(nameof(UpperCriticalSoilMoisture),
                    $"{nameof(UpperCriticalSoilMoisture)} field cannot has value lower or equal to field {nameof(LowerCriticalSoilMoisture)}"));
            }

            if (LowerCriticalSoilMoisture >= UpperCriticalSoilMoisture)
            {
                errors.Add(new ValidationError(nameof(LowerCriticalSoilMoisture),
                    $"{nameof(LowerCriticalSoilMoisture)} field cannot has value higher or equal to field {nameof(UpperCriticalSoilMoisture)}"));
            }

            return ConvertToArray(errors);
        }

        public static SetConfigCommand FromHashtable(Hashtable obj)
        {
            var command = new SetConfigCommand(
                obj.GetString(nameof(CorrelationId)),
                obj.GetShort(nameof(DayVegetationMaxOptimalTemperature)),
                obj.GetShort(nameof(DayVegetationMinOptimalTemperature)),
                obj.GetShort(nameof(NightVegetationMaxOptimalTemperature)),
                obj.GetShort(nameof(NightVegetationMinOptimalTemperature)),
                obj.GetShort(nameof(MaxOptimalHumidity)),
                obj.GetShort(nameof(MinOptimalHumidity)),
                obj.GetShort(nameof(UpperCriticalTemperature)),
                obj.GetShort(nameof(LowerCriticalTemperature)),
                obj.GetShort(nameof(UpperCriticalHumidity)),
                obj.GetShort(nameof(LowerCriticalHumidity)),
                obj.GetShort(nameof(UpperCriticalSoilMoisture)),
                obj.GetShort(nameof(LowerCriticalSoilMoisture)));
            return command;
        }

        public class Factory : ICommandFactory
        {
            public ICommand Create(Hashtable obj)
            {
                return FromHashtable(obj);
            }
        }
    }
}