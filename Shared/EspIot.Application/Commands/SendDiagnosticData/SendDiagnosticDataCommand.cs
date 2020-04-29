using System.Collections;
using EspIot.Application.Interfaces;
using EspIot.Core.Extensions;

namespace EspIot.Application.Commands.SendDiagnosticData
{
    public class SendDiagnosticDataCommand : CommandBase
    {
        public SendDiagnosticDataCommand(string correlationId) : base(correlationId)
        {
        }

        public override string PartitionKey { get; } = nameof(SendDiagnosticDataCommand);

        public static SendDiagnosticDataCommand FromHashtable(Hashtable obj)
        {
            return new SendDiagnosticDataCommand(obj.GetString(nameof(CorrelationId)));
        }

        public class Factory : ICommandFactory
        {
            ICommand ICommandFactory.Create(Hashtable obj)
            {
                return FromHashtable(obj);
            }
        }
    }
}