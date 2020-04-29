using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;

namespace EspIot.Application.Services
{
    public interface IDiagnosticService : IService
    {
        DiagnosticEvent GetDiagnosticDataEvent();
    }
}