using System.Collections;
using EspIot.Application.Commands;
using EspIot.Application.Interfaces;
using EspIot.Core.Extensions;

namespace GreenhouseController.Application.Commands.AbortIrrigation
{
    public class AbortIrrigationCommand: CommandBase
    {
        public AbortIrrigationCommand(string correlationId) : base(correlationId)
        {
        }

        public override string PartitionKey { get; } = nameof(AbortIrrigationCommand);

        public static AbortIrrigationCommand FromHashtable(Hashtable obj)
        {
            return new AbortIrrigationCommand(
                obj.GetString(nameof(CorrelationId)));
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
