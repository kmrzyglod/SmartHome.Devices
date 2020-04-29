using System.Collections;

namespace EspIot.Application.Interfaces
{
    public interface ICommandsFactory
    {
        ICommand Create(string commandName, Hashtable payload);
    }
}
