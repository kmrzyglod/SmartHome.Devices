using System;
using Windows.Devices.I2c;

namespace EspIot.Core.I2c
{
    public class I2CTransferException : Exception
    {
        public I2cTransferStatus TransferStatus {get; } 
        public I2CTransferException(I2cTransferStatus transferStatus)
        {
        }

        public I2CTransferException(string message, I2cTransferStatus transferStatus)
            : base(message)
        {
        }

        public I2CTransferException(string message, I2cTransferStatus transferStatus, Exception inner)
            : base(message, inner)
        {
        }
    }
}
