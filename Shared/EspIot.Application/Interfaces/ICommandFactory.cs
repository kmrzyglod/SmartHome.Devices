using System.Collections;

namespace EspIot.Application.Interfaces
{
    public interface ICommandFactory
    {
        ICommand Create(Hashtable obj);
    }
}
