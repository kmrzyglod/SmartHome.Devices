using System.Collections;

namespace EspIot.Core.Messaging.Interfaces
{
    public interface ICommandsFactory
    {
        ICommand CreateCommand(string commandName, Hashtable payload);
    }
}
