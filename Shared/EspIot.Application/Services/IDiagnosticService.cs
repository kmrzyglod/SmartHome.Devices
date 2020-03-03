using EspIot.Application.Events.Outbound;
using EspIot.Core.Messaging.Interfaces;

namespace EspIot.Application.Services
{
    public interface IDiagnosticService : IService
    {
        DiagnosticEvent GetDiagnosticDataEvent();
    }
}