namespace EspIot.Drivers.ReedSwitch.Enums
{
    public enum ReedShiftState
    {
        Opened = 1,
        Closed = 0
    }

    public static class ReedShiftStateExtensions
    {
        public static bool ToBool(this ReedShiftState state)
        {
            return state == ReedShiftState.Opened ? true : false;
        }
    }
}

