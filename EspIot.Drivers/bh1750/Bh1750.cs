using EspIot.Core.I2c;
using System;
using System.Threading;
using Windows.Devices.I2c;

namespace EspIot.Drivers.Bh1750
{
    public enum MeasurementMode
    {
        ContinuouslyHighResolutionMode = 0x10,
        ContinuouslyHighResolutionMode2 = 0x11,
        ContinuouslyLowResolutionMode = 0x13,
        OneTimeHighResolutionMode = 0x20,
        OneTimeHighResolutionMode2 = 0x21,
        OneTimeLowResolutionMode = 0x23
    }

    public enum PinConnection
    {
        PIN_HIGH = 0x5C,
        PIN_LOW = 0x23
    }

    public class Bh1750
    {
        private readonly string _i2CControllerName;
        private readonly PinConnection _pinConnection;
        private readonly I2cDevice _device = null;

        public Bh1750(string i2CControllerName, 
            PinConnection pinConnection = PinConnection.PIN_LOW, 
            MeasurementMode measurementMode = MeasurementMode.ContinuouslyHighResolutionMode2)
        {
            _pinConnection = pinConnection;
            _i2CControllerName = i2CControllerName;
            try
            {
                var settings = new I2cConnectionSettings((byte)_pinConnection);
                settings.BusSpeed = I2cBusSpeed.StandardMode;
                settings.SharingMode = I2cSharingMode.Shared;
                string aqs = I2cDevice.GetDeviceSelector(_i2CControllerName);
                _device = I2cDevice.FromId(_i2CControllerName, settings);
                if (_device == null)
                {
                    Console.WriteLine("Device not found");
                }

                SetMode(measurementMode);

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message + "\n" + e.StackTrace);
                throw;
            }

        }

        public void SetMode(MeasurementMode measurementMode)
        {
            _device.Write(new byte[] { (byte)measurementMode });
            Thread.Sleep(10);
        }

        public int GetLightLevelInLux()
        {
            byte[] result = _device.ReadBytes((byte)_pinConnection, 2);
            int lightLevel = result[0] << 8 | result[1];

            return (int)(lightLevel / 1.2f);
        }

        public void PowerOff()
        {
            _device.Write(new byte[] { 0x00 });
        }

        public void PowerOn()
        {
            _device.Write(new byte[] { 0x01 });
        }

        public void Reset()
        {
            _device.Write(new byte[] { 0x07 });
        }
    }
}
