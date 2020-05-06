using System.Collections;
using EspIot.Application.Interfaces;

namespace WindowsController.Infrastructure.Factory
{
    public class CommandHandlersFactory: ICommandHandlersFactory
    {
        private static Hashtable _mappings { get; set; }

        public ICommandHandler Get(string commandName)
        {
            throw new System.NotImplementedException();
        }
    }
}
