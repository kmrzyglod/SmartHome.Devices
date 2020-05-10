using System;
using System.Threading;

namespace WindowsController.Firmware
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
