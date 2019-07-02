// MIT License
// Original Source: https://github.com/ms-iot/adafruitsample/tree/master/Lesson_203/FullSolution

using GreenhouseController.Core.I2c;
using System;
using System.Threading;
using Windows.Devices.I2c;


namespace GreenhouseController.Drivers.Bme280
{
    public class BME280_CalibrationData
    {
        //BME280 Registers
        public ushort dig_T1 { get; set; }
        public short dig_T2 { get; set; }
        public short dig_T3 { get; set; }

        public ushort dig_P1 { get; set; }
        public short dig_P2 { get; set; }
        public short dig_P3 { get; set; }
        public short dig_P4 { get; set; }
        public short dig_P5 { get; set; }
        public short dig_P6 { get; set; }
        public short dig_P7 { get; set; }
        public short dig_P8 { get; set; }
        public short dig_P9 { get; set; }

        public byte dig_H1 { get; set; }
        public short dig_H2 { get; set; }
        public byte dig_H3 { get; set; }
        public short dig_H4 { get; set; }
        public short dig_H5 { get; set; }
        public sbyte dig_H6 { get; set; }
    }


    public class Bme280
    {
        const byte BME280_Address = 0x76;
        const byte BME280_Signature = 0x60;

        enum eRegisters : byte
        {
            BME280_REGISTER_DIG_T1 = 0x88,
            BME280_REGISTER_DIG_T2 = 0x8A,
            BME280_REGISTER_DIG_T3 = 0x8C,

            BME280_REGISTER_DIG_P1 = 0x8E,
            BME280_REGISTER_DIG_P2 = 0x90,
            BME280_REGISTER_DIG_P3 = 0x92,
            BME280_REGISTER_DIG_P4 = 0x94,
            BME280_REGISTER_DIG_P5 = 0x96,
            BME280_REGISTER_DIG_P6 = 0x98,
            BME280_REGISTER_DIG_P7 = 0x9A,
            BME280_REGISTER_DIG_P8 = 0x9C,
            BME280_REGISTER_DIG_P9 = 0x9E,

            BME280_REGISTER_DIG_H1 = 0xA1,
            BME280_REGISTER_DIG_H2 = 0xE1,
            BME280_REGISTER_DIG_H3 = 0xE3,
            BME280_REGISTER_DIG_H4_L = 0xE4,
            BME280_REGISTER_DIG_H4_H = 0xE5,
            BME280_REGISTER_DIG_H5_L = 0xE5,
            BME280_REGISTER_DIG_H5_H = 0xE6,
            BME280_REGISTER_DIG_H6 = 0xE7,

            BME280_REGISTER_CHIPID = 0xD0,
            BME280_REGISTER_SOFTRESET = 0xE0,

            BME280_REGISTER_CONTROLHUMID = 0xF2,
            BME280_REGISTER_STATUS = 0xF3,
            BME280_REGISTER_CONTROL = 0xF4,
            BME280_REGISTER_CONFIG = 0xF5,

            BME280_REGISTER_PRESSUREDATA_MSB = 0xF7,
            BME280_REGISTER_PRESSUREDATA_LSB = 0xF8,
            BME280_REGISTER_PRESSUREDATA_XLSB = 0xF9, // bits <7:4>

            BME280_REGISTER_TEMPDATA_MSB = 0xFA,
            BME280_REGISTER_TEMPDATA_LSB = 0xFB,
            BME280_REGISTER_TEMPDATA_XLSB = 0xFC, // bits <7:4>

            BME280_REGISTER_HUMIDDATA_MSB = 0xFD,
            BME280_REGISTER_HUMIDDATA_LSB = 0xFE,
        };

        // Enables 2-wire I2C interface when set to ‘0’
        public enum interface_mode_e : byte
        {
            i2c = 0,
            spi = 1
        };

        // t_sb standby options - effectively the gap between automatic measurements 
        // when in "normal" mode
        public enum standbySettings_e : byte
        {
            tsb_0p5ms = 0,
            tsb_62p5ms = 1,
            tsb_125ms = 2,
            tsb_250ms = 3,
            tsb_500ms = 4,
            tsb_1000ms = 5,
            tsb_10ms = 6,
            tsb_20ms = 7
        };


        // sensor modes, it starts off in sleep mode on power on
        // forced is to take a single measurement now
        // normal takes measurements reqularly automatically
        public enum mode_e : byte
        {
            smSleep = 0,
            smForced = 1,
            smNormal = 3
        };


        // Filter coefficients
        // higher numbers slow down changes, such as slamming doors
        public enum filterCoefficient_e : byte
        {
            fc_off = 0,
            fc_2 = 1,
            fc_4 = 2,
            fc_8 = 3,
            fc_16 = 4
        };


        // Oversampling options for humidity
        // Oversampling reduces the noise from the sensor
        public enum oversampling_e : byte
        {
            osSkipped = 0,
            os1x = 1,
            os2x = 2,
            os4x = 3,
            os8x = 4,
            os16x = 5
        };


        private readonly string _i2cControllerName;
        private I2cDevice _bme280 = null;
        private BME280_CalibrationData _calibrationData;

        // Value hold sensor operation parameters
        private byte _intMode = (byte)interface_mode_e.i2c;
        private byte _tSb;
        private byte _mode;
        private byte _filter;
        private byte _osrsP;
        private byte _osrsT;
        private byte _osrsH;

        public Bme280(string i2CControllerName, standbySettings_e t_sb = standbySettings_e.tsb_1000ms,
                      mode_e mode = mode_e.smNormal,
                      filterCoefficient_e filter = filterCoefficient_e.fc_16,
                      oversampling_e osrs_p = oversampling_e.os4x,
                      oversampling_e osrs_t = oversampling_e.os2x,
                      oversampling_e osrs_h = oversampling_e.os8x)
        {
            
            _i2cControllerName = i2CControllerName;
            _tSb = (byte)t_sb;
            _mode = (byte)mode;
            _filter = (byte)filter;
            _osrsP = (byte)osrs_p;
            _osrsT = (byte)osrs_t;
            _osrsH = (byte)osrs_h;
        }

        public void Initialize()
        {
            Console.WriteLine("BME280::Initialize");
            try
            {
               
                var settings = new I2cConnectionSettings(BME280_Address);
                settings.BusSpeed = I2cBusSpeed.StandardMode;
                settings.SharingMode = I2cSharingMode.Shared;
                string aqs = I2cDevice.GetDeviceSelector(_i2cControllerName);
                _bme280 = I2cDevice.FromId(_i2cControllerName, settings);
                
                if (_bme280 == null)
                {
                    Console.WriteLine("Device not found");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message + "\n" + e.StackTrace);
                throw;
            }

            byte[] readChipID = new byte[] { (byte)eRegisters.BME280_REGISTER_CHIPID };
            byte[] ReadBuffer = new byte[] { 0xFF };

            //Read the device signature
            _bme280.WriteReadPartial(readChipID, ReadBuffer);
            Console.WriteLine("BME280 Signature: " + ReadBuffer[0].ToString());

            //Verify the device signature
            if (ReadBuffer[0] != BME280_Signature)
            {
                Console.WriteLine("BME280::Begin Signature Mismatch.");
                return;
            }

            //Set configuration registers
            WriteConfigRegister();
            WriteControlMeasurementRegister();
            WriteControlRegisterHumidity();

            //Set configuration registers again to ensure configuration of humidity
            WriteConfigRegister();
            WriteControlMeasurementRegister();
            WriteControlRegisterHumidity();

            //Read the coefficients table
            _calibrationData = ReadCoefficeints();

            //Dummy read temp to setup t_fine
            ReadTemperature();
        }


        //Method to write the config register (default 16)
        //000  100  00 
        // ↑  ↑   ↑I2C mode
        // ↑  ↑Filter coefficient = 16
        // ↑t_sb = 0.5ms
        private void WriteConfigRegister()
        {
            byte value = (byte)(_intMode + (_filter << 2) + (_tSb << 5));
            byte[] WriteBuffer = new byte[] { (byte)eRegisters.BME280_REGISTER_CONFIG, value };
            _bme280.Write(WriteBuffer);
            return;
        }

        //Method to write the control measurment register (default 87)
        //010  101  11 
        // ↑  ↑   ↑ mode
        // ↑  ↑ Pressure oversampling
        // ↑ Temperature oversampling
        private void WriteControlMeasurementRegister()
        {
            byte value = (byte)(_mode + (_osrsP << 2) + (_osrsT << 5));
            byte[] WriteBuffer = new byte[] { (byte)eRegisters.BME280_REGISTER_CONTROL, value };
            _bme280.Write(WriteBuffer);
            return;
        }

        //Method to write the humidity control register (default 01)
        private void WriteControlRegisterHumidity()
        {
            byte value = _osrsH;
            byte[] WriteBuffer = new byte[] { (byte)eRegisters.BME280_REGISTER_CONTROLHUMID, value };
            _bme280.Write(WriteBuffer);
            return;
        }

        //Method to read the caliberation data from the registers
        private BME280_CalibrationData ReadCoefficeints()
        {
            // 16 bit calibration data is stored as Little Endian, the helper method will do the byte swap.
            _calibrationData = new BME280_CalibrationData();


            // Read temperature calibration data
            _calibrationData.dig_T1 = (ushort)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_T1, 2);
            _calibrationData.dig_T2 = (short)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_T2, 2);
            _calibrationData.dig_T3 = (short)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_T3, 2);

            // Read presure calibration data
            _calibrationData.dig_P9 = (short)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_P9, 2);
            _calibrationData.dig_P8 = (short)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_P8, 2);
            _calibrationData.dig_P7 = (short)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_P7, 2);
            _calibrationData.dig_P6 = (short)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_P6, 2);
            _calibrationData.dig_P5 = (short)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_P5, 2);
            _calibrationData.dig_P4 = (short)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_P4, 2);
            _calibrationData.dig_P3 = (short)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_P3, 2);
            _calibrationData.dig_P2 = (short)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_P2, 2);
            _calibrationData.dig_P1 = (ushort)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_P1, 2);

            // Read humidity calibration data

            _calibrationData.dig_H1 = (byte)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_H1);
            _calibrationData.dig_H2 = (short)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_H2, 2);
            _calibrationData.dig_H3 = (byte)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_H3);
            short e4 = (short)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_H4_L);    // Read 0xE4
            short e5 = (short)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_H4_H);    // Read 0xE5
            short e6 = (short)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_H5_H);    // Read 0xE6
            _calibrationData.dig_H6 = (sbyte)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_DIG_H6);

            _calibrationData.dig_H4 = (short)((e4 << 4) + (e5 & 0x0F));
            _calibrationData.dig_H5 = (short)((e5 >> 4) + (e6 << 4));

            return _calibrationData;
        }


        //t_fine carries fine temperature as global value
        int t_fine;

        //Method to return the temperature in DegC. Resolution is 0.01 DegC. Output value of “51.23” equals 51.23 DegC.
        private double BME280_compensate_T_double(int adc_T)
        {
            int var1, var2;
            double T;
            var1 = ((((adc_T >> 3) - (_calibrationData.dig_T1 << 1))) * (_calibrationData.dig_T2)) >> 11;
            var2 = (((((adc_T >> 4) - (_calibrationData.dig_T1)) * ((adc_T >> 4) - (_calibrationData.dig_T1))) >> 12) * (_calibrationData.dig_T3)) >> 14;

            t_fine = var1 + var2;
            T = (t_fine * 5 + 128) >> 8;
            return T / 100;
        }


        //Method to returns the pressure in Pa, in Q24.8 format (24 integer bits and 8 fractional bits).
        //Output value of “24674867” represents 24674867/256 = 96386.2 Pa = 963.862 hPa
        private long BME280_compensate_P_Int64(int adc_P)
        {
            long var1, var2, p;

            //The pressure is calculated using the compensation formula in the BME280 datasheet
            var1 = (long)t_fine - 128000;
            var2 = var1 * var1 * _calibrationData.dig_P6;
            var2 = var2 + ((var1 * _calibrationData.dig_P5) << 17);
            var2 = var2 + ((long)_calibrationData.dig_P4 << 35);
            var1 = ((var1 * var1 * _calibrationData.dig_P3) >> 8) + ((var1 * _calibrationData.dig_P2) << 12);
            var1 = (((long)1 << 47) + var1) * _calibrationData.dig_P1 >> 33;
            if (var1 == 0)
            {
                Console.WriteLine("BME280_compensate_P_Int64 Jump out to avoid / 0");
                return 0; //Avoid exception caused by division by zero
            }
            //Perform calibration operations as per datasheet: 
            p = 1048576 - adc_P;
            p = (((p << 31) - var2) * 3125) / var1;
            var1 = ((long)_calibrationData.dig_P9 * (p >> 13) * (p >> 13)) >> 25;
            var2 = ((long)_calibrationData.dig_P8 * p) >> 19;
            p = ((p + var1 + var2) >> 8) + ((long)_calibrationData.dig_P7 << 4);
            return p;
        }


        // Returns humidity in %rH as as double. Output value of “46.332” represents 46.332 %rH
        private double BME280_compensate_H_double(int adc_H)
        {
            double var_H;

            var_H = t_fine - 76800.0;
            var_H = (adc_H - (_calibrationData.dig_H4 * 64.0 + _calibrationData.dig_H5 / 16384.0 * var_H)) *
                _calibrationData.dig_H2 / 65536.0 * (1.0 + _calibrationData.dig_H6 / 67108864.0 * var_H *
                (1.0 + _calibrationData.dig_H3 / 67108864.0 * var_H));
            var_H = var_H * (1.0 - _calibrationData.dig_H1 * var_H / 524288.0);

            if (var_H > 100.0)
            {
                Console.WriteLine("BME280_compensate_H_double Jump out to 100%");
                var_H = 100.0;
            }
            else if (var_H < 0.0)
            {
                Console.WriteLine("BME280_compensate_H_double Jump under 0%");
                var_H = 0.0;
            }

            return var_H;
        }


        public float ReadTemperature()
        {
            //Read the MSB, LSB and bits 7:4 (XLSB) of the temperature from the BME280 registers
            byte tmsb = (byte)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_TEMPDATA_MSB);
            byte tlsb = (byte)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_TEMPDATA_LSB);
            byte txlsb = (byte)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_TEMPDATA_XLSB); // bits 7:4

            //Combine the values into a 32-bit integer
            int t = ((tmsb << 12) | (tlsb << 4) | (txlsb >> 4));

            //Convert the raw value to the temperature in degC
            double temp = BME280_compensate_T_double(t);

            //Return the temperature as a float value
            return (float)temp;
        }

        public float ReadPreasure()
        {
            //Read the MSB, LSB and bits 7:4 (XLSB) of the pressure from the BME280 registers
            byte pmsb = (byte)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_PRESSUREDATA_MSB);
            byte plsb = (byte)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_PRESSUREDATA_LSB);
            byte pxlsb = (byte)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_PRESSUREDATA_XLSB); // bits 7:4

            //Combine the values into a 32-bit integer
            int p = (pmsb << 12) | (plsb << 4) | (pxlsb >> 4);

            //Convert the raw value to the pressure in Pa
            long pres = BME280_compensate_P_Int64(p);

            //Return the pressure as a float value
            return ((float)pres) / 256;
        }

        public float ReadHumidity()
        {
            //Read the MSB and LSB of the humidity from the BME280 registers
            byte hmsb = (byte)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_HUMIDDATA_MSB);
            byte hlsb = (byte)_bme280.ReadValue((byte)eRegisters.BME280_REGISTER_HUMIDDATA_LSB);

            //Combine the values into a 32-bit integer
            int h = (hmsb << 8) + hlsb;

            //Convert the raw value to the humidity in %
            double humidity = BME280_compensate_H_double(h);

            //Return the humidity as a float value
            return (float)humidity;
        }
    }
}