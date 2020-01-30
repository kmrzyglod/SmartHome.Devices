using Windows.Devices.I2c;

namespace EspIot.Core.I2c
{
    public static class I2CExtensions
    {
        //Method to read bytes from registers
        public static int ReadValue(this I2cDevice device, byte register, short numOfBytes = 1, short retries = 3)
        {
            var readBuffer = ReadBytes(device, register, numOfBytes, retries);
            int value = readBuffer[0];
            for (short j = 1; j < numOfBytes; j++)
            {
                value += readBuffer[j] << (8 * j);
            }
            return value;
        }

        //Method to read bytes from registers
        public static byte[] ReadBytes(this I2cDevice device, byte register, short numOfBytes = 1, short retries = 3)
        {
            byte[] writeBuffer = new byte[] { 0x00 };
            byte[] readBuffer = new byte[numOfBytes];

            writeBuffer[0] = register;
            var status = I2cTransferStatus.UnknownError;
            for (short i = 0; i < retries; i++)
            {
                status = device.WriteReadPartial(writeBuffer, readBuffer).Status;
                if (status == I2cTransferStatus.FullTransfer)
                {
                    break;
                }
            }

            if (status != I2cTransferStatus.FullTransfer)
            {
                throw new I2CTransferException("Error during read byte", status);
            }

            return readBuffer;
        }
    }
}
