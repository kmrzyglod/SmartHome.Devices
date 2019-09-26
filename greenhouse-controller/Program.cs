using System.Threading;

namespace GreenhouseController
{
    public class Program
    {
        public static void Main()
        {
            Bootloader.StartServices();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
