namespace EspIot.Application.Interfaces
{
    public interface IService
    {
        //Starting and stopping service should be non-blocking 
        void Start();
        void Stop(); 
        bool IsRunning();
    }
}
