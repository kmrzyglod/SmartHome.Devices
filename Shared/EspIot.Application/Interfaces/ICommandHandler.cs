
namespace EspIot.Application.Interfaces
{
    public interface ICommandHandler
    {
        void Handle(ICommand command);
    }
}
