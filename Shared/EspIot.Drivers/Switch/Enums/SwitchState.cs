namespace EspIot.Drivers.Switch.Enums
{
    public enum SwitchState
    {
        Opened = 1,
        Closed = 0
    }

    public static class ShiftStateExtensions
    {
        public static bool ToBool(this SwitchState state)
        {
            return state == SwitchState.Opened ? true : false;
        }
    }
}

