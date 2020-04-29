using System.Collections;
using EspIot.Core.Extensions;
using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Interfaces;

namespace EspIot.Application.Commands.Ping
{
    public class PingCommand: CommandBase
    {
        public PingCommand(string correlationId) : base((string) correlationId)
        {
        }

        public override string PartitionKey { get; } = "Ping";

        public static PingCommand FromHashtable(Hashtable obj)
        {
            return new PingCommand(obj.GetString(nameof(CorrelationId)));
        }
        
        public class Factory: ICommandFactory
        {
            public ICommand Create(Hashtable obj)
            {
                return FromHashtable(obj);
            }
        }
    }
}
