using System;
using System.Threading;

namespace GreenhouseController
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
