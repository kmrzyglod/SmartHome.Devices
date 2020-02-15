using System.Collections;

namespace EspIot.Core.Messaging.Interfaces
{
    public interface ICommandFactory
    {
        ICommand Create(Hashtable obj);
    }
}
