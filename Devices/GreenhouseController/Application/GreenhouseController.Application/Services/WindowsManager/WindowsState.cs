namespace GreenhouseController.Application.Services.WindowsManager
{
    public class WindowsState
    {
        public WindowsState(bool isWindow1Opened, bool isWindow2Opened, bool isDoorsOpened)
        {
            IsWindow1Opened = isWindow1Opened;
            IsWindow2Opened = isWindow2Opened;
            IsDoorsOpened = isDoorsOpened;
        }

        public bool IsWindow1Opened { get; }
        public bool IsWindow2Opened { get; }
        public bool IsDoorsOpened { get; }
    }
}
