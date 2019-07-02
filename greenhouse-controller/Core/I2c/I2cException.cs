using System;
using Windows.Devices.I2c;

namespace GreenhouseController.Core.I2c
{
    public class I2cTransferException : Exception
    {
        public I2cTransferStatus _transferStatus {get; } 
        public I2cTransferException(I2cTransferStatus transferStatus)
        {
        }

        public I2cTransferException(string message, I2cTransferStatus transferStatus)
            : base(message)
        {
        }

        public I2cTransferException(string message, I2cTransferStatus transferStatus, Exception inner)
            : base(message, inner)
        {
        }
    }
}
