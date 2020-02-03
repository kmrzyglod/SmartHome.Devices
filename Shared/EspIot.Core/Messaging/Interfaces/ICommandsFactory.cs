using System.Collections;

namespace EspIot.Core.Messaging.Interfaces
{
    public interface ICommandsFactory
    {
        ICommand Create(string commandName, Hashtable payload);
    }
}
