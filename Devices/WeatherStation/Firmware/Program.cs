using System.Threading;

namespace WeatherStation.Firmware
{
    public class Program
    {
        public static void Main()
        {
            Bootloader.StartServices();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
