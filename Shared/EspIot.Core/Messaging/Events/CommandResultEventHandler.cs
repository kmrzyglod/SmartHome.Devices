using EspIot.Core.Messaging.Interfaces;

namespace EspIot.Core.Messaging.Events
{
    public delegate void CommandResultEventHandler(object sender, ICommandResultEvent e);
}
