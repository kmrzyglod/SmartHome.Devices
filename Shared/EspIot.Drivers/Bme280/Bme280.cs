// MIT License
// Original Source: https://github.com/ms-iot/adafruitsample/tree/master/Lesson_203/FullSolution

using EspIot.Core.I2c;
using System;
using Windows.Devices.I2c;

namespace EspIot.Drivers.Bme280
{
    public class Bme280CalibrationData
    {
        //BME280 Registers
        public ushort DigT1 { get; set; }
        public short DigT2 { get; set; }
        public short DigT3 { get; set; }

        public ushort DigP1 { get; set; }
        public short DigP2 { get; set; }
        public short DigP3 { get; set; }
        public short DigP4 { get; set; }
        public short DigP5 { get; set; }
        public short DigP6 { get; set; }
        public short DigP7 { get; set; }
        public short DigP8 { get; set; }
        public short DigP9 { get; set; }

        public byte DigH1 { get; set; }
        public short DigH2 { get; set; }
        public byte DigH3 { get; set; }
        public short DigH4 { get; set; }
        public short DigH5 { get; set; }
        public sbyte DigH6 { get; set; }
    }


    public class Bme280
    {
        const byte Bme280Address = 0x76;
        const byte Bme280Signature = 0x60;

        enum ERegisters : byte
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
        public enum InterfaceModeE : byte
        {
            I2_C = 0,
            SPI = 1
        };

        // t_sb standby options - effectively the gap between automatic measurements 
        // when in "normal" mode
        public enum StandbySettingsE : byte
        {
            TSB_0_P5_MS = 0,
            TSB_62_P5_MS = 1,
            TSB_125_MS = 2,
            TSB_250_MS = 3,
            TSB_500_MS = 4,
            TSB_1000_MS = 5,
            TSB_10_MS = 6,
            TSB_20_MS = 7
        };


        // sensor modes, it starts off in sleep mode on power on
        // forced is to take a single measurement now
        // normal takes measurements reqularly automatically
        public enum ModeE : byte
        {
            SM_SLEEP = 0,
            SM_FORCED = 1,
            SM_NORMAL = 3
        };


        // Filter coefficients
        // higher numbers slow down changes, such as slamming doors
        public enum FilterCoefficientE : byte
        {
            FC_OFF = 0,
            FC_2 = 1,
            FC_4 = 2,
            FC_8 = 3,
            FC_16 = 4
        };


        // Oversampling options for humidity
        // Oversampling reduces the noise from the sensor
        public enum OversamplingE : byte
        {
            OS_SKIPPED = 0,
            OS1_X = 1,
            OS2_X = 2,
            OS4_X = 3,
            OS8_X = 4,
            OS16_X = 5
        };


        private readonly string _i2CControllerName;
        private I2cDevice _bme280 = null;
        private Bme280CalibrationData _calibrationData;

        // Value hold sensor operation parameters
        private readonly byte _intMode = (byte)InterfaceModeE.I2_C;
        private readonly byte _tSb;
        private readonly byte _mode;
        private readonly byte _filter;
        private readonly byte _osrsP;
        private readonly byte _osrsT;
        private readonly byte _osrsH;

        public Bme280(string i2CControllerName, StandbySettingsE tSb = StandbySettingsE.TSB_1000_MS,
                      ModeE mode = ModeE.SM_NORMAL,
                      FilterCoefficientE filter = FilterCoefficientE.FC_16,
                      OversamplingE osrsP = OversamplingE.OS4_X,
                      OversamplingE osrsT = OversamplingE.OS2_X,
                      OversamplingE osrsH = OversamplingE.OS8_X)
        {
            
            _i2CControllerName = i2CControllerName;
            _tSb = (byte)tSb;
            _mode = (byte)mode;
            _filter = (byte)filter;
            _osrsP = (byte)osrsP;
            _osrsT = (byte)osrsT;
            _osrsH = (byte)osrsH;
        }

        public Bme280 Initialize()
        {
            Console.WriteLine("BME280::Initialize");
            try
            {
               
                var settings = new I2cConnectionSettings(Bme280Address);
                settings.BusSpeed = I2cBusSpeed.StandardMode;
                settings.SharingMode = I2cSharingMode.Shared;
                string aqs = I2cDevice.GetDeviceSelector(_i2CControllerName);
                _bme280 = I2cDevice.FromId(_i2CControllerName, settings);
                
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

            byte[] readChipId = new byte[] { (byte)ERegisters.BME280_REGISTER_CHIPID };
            byte[] readBuffer = new byte[] { 0xFF };

            //Read the device signature
            _bme280.WriteReadPartial(readChipId, readBuffer);
            Console.WriteLine("BME280 Signature: " + readBuffer[0].ToString());

            //Verify the device signature
            if (readBuffer[0] != Bme280Signature)
            {
                Console.WriteLine("BME280::Begin Signature Mismatch.");
                return this;
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

            return this;
        }


        //Method to write the config register (default 16)
        //000  100  00 
        // ↑  ↑   ↑I2C mode
        // ↑  ↑Filter coefficient = 16
        // ↑t_sb = 0.5ms
        private void WriteConfigRegister()
        {
            byte value = (byte)(_intMode + (_filter << 2) + (_tSb << 5));
            byte[] writeBuffer = new byte[] { (byte)ERegisters.BME280_REGISTER_CONFIG, value };
            _bme280.Write(writeBuffer);
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
            byte[] writeBuffer = new byte[] { (byte)ERegisters.BME280_REGISTER_CONTROL, value };
            _bme280.Write(writeBuffer);
            return;
        }

        //Method to write the humidity control register (default 01)
        private void WriteControlRegisterHumidity()
        {
            byte value = _osrsH;
            byte[] writeBuffer = new byte[] { (byte)ERegisters.BME280_REGISTER_CONTROLHUMID, value };
            _bme280.Write(writeBuffer);
            return;
        }

        //Method to read the caliberation data from the registers
        private Bme280CalibrationData ReadCoefficeints()
        {
            // 16 bit calibration data is stored as Little Endian, the helper method will do the byte swap.
            _calibrationData = new Bme280CalibrationData();


            // Read temperature calibration data
            _calibrationData.DigT1 = (ushort)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_T1, 2);
            _calibrationData.DigT2 = (short)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_T2, 2);
            _calibrationData.DigT3 = (short)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_T3, 2);

            // Read presure calibration data
            _calibrationData.DigP9 = (short)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_P9, 2);
            _calibrationData.DigP8 = (short)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_P8, 2);
            _calibrationData.DigP7 = (short)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_P7, 2);
            _calibrationData.DigP6 = (short)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_P6, 2);
            _calibrationData.DigP5 = (short)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_P5, 2);
            _calibrationData.DigP4 = (short)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_P4, 2);
            _calibrationData.DigP3 = (short)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_P3, 2);
            _calibrationData.DigP2 = (short)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_P2, 2);
            _calibrationData.DigP1 = (ushort)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_P1, 2);

            // Read humidity calibration data

            _calibrationData.DigH1 = (byte)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_H1);
            _calibrationData.DigH2 = (short)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_H2, 2);
            _calibrationData.DigH3 = (byte)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_H3);
            short e4 = (short)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_H4_L);    // Read 0xE4
            short e5 = (short)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_H4_H);    // Read 0xE5
            short e6 = (short)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_H5_H);    // Read 0xE6
            _calibrationData.DigH6 = (sbyte)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_DIG_H6);

            _calibrationData.DigH4 = (short)((e4 << 4) + (e5 & 0x0F));
            _calibrationData.DigH5 = (short)((e5 >> 4) + (e6 << 4));

            return _calibrationData;
        }


        //t_fine carries fine temperature as global value
        int _tFine;

        //Method to return the temperature in DegC. Resolution is 0.01 DegC. Output value of “51.23” equals 51.23 DegC.
        private double BME280_compensate_T_double(int adcT)
        {
            int var1, var2;
            double T;
            var1 = ((((adcT >> 3) - (_calibrationData.DigT1 << 1))) * (_calibrationData.DigT2)) >> 11;
            var2 = (((((adcT >> 4) - (_calibrationData.DigT1)) * ((adcT >> 4) - (_calibrationData.DigT1))) >> 12) * (_calibrationData.DigT3)) >> 14;

            _tFine = var1 + var2;
            T = (_tFine * 5 + 128) >> 8;
            return T / 100;
        }


        //Method to returns the pressure in Pa, in Q24.8 format (24 integer bits and 8 fractional bits).
        //Output value of “24674867” represents 24674867/256 = 96386.2 Pa = 963.862 hPa
        private long BME280_compensate_P_Int64(int adcP)
        {
            long var1, var2, p;

            //The pressure is calculated using the compensation formula in the BME280 datasheet
            var1 = (long)_tFine - 128000;
            var2 = var1 * var1 * _calibrationData.DigP6;
            var2 = var2 + ((var1 * _calibrationData.DigP5) << 17);
            var2 = var2 + ((long)_calibrationData.DigP4 << 35);
            var1 = ((var1 * var1 * _calibrationData.DigP3) >> 8) + ((var1 * _calibrationData.DigP2) << 12);
            var1 = (((long)1 << 47) + var1) * _calibrationData.DigP1 >> 33;
            if (var1 == 0)
            {
                Console.WriteLine("BME280_compensate_P_Int64 Jump out to avoid / 0");
                return 0; //Avoid exception caused by division by zero
            }
            //Perform calibration operations as per datasheet: 
            p = 1048576 - adcP;
            p = (((p << 31) - var2) * 3125) / var1;
            var1 = ((long)_calibrationData.DigP9 * (p >> 13) * (p >> 13)) >> 25;
            var2 = ((long)_calibrationData.DigP8 * p) >> 19;
            p = ((p + var1 + var2) >> 8) + ((long)_calibrationData.DigP7 << 4);
            return p;
        }


        // Returns humidity in %rH as as double. Output value of “46.332” represents 46.332 %rH
        private double BME280_compensate_H_double(int adcH)
        {
            double varH;

            varH = _tFine - 76800.0;
            varH = (adcH - (_calibrationData.DigH4 * 64.0 + _calibrationData.DigH5 / 16384.0 * varH)) *
                _calibrationData.DigH2 / 65536.0 * (1.0 + _calibrationData.DigH6 / 67108864.0 * varH *
                (1.0 + _calibrationData.DigH3 / 67108864.0 * varH));
            varH = varH * (1.0 - _calibrationData.DigH1 * varH / 524288.0);

            if (varH > 100.0)
            {
                Console.WriteLine("BME280_compensate_H_double Jump out to 100%");
                varH = 100.0;
            }
            else if (varH < 0.0)
            {
                Console.WriteLine("BME280_compensate_H_double Jump under 0%");
                varH = 0.0;
            }

            return varH;
        }


        public float ReadTemperature()
        {
            //Read the MSB, LSB and bits 7:4 (XLSB) of the temperature from the BME280 registers
            byte tmsb = (byte)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_TEMPDATA_MSB);
            byte tlsb = (byte)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_TEMPDATA_LSB);
            byte txlsb = (byte)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_TEMPDATA_XLSB); // bits 7:4

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
            byte pmsb = (byte)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_PRESSUREDATA_MSB);
            byte plsb = (byte)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_PRESSUREDATA_LSB);
            byte pxlsb = (byte)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_PRESSUREDATA_XLSB); // bits 7:4

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
            byte hmsb = (byte)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_HUMIDDATA_MSB);
            byte hlsb = (byte)_bme280.ReadValue((byte)ERegisters.BME280_REGISTER_HUMIDDATA_LSB);

            //Combine the values into a 32-bit integer
            int h = (hmsb << 8) + hlsb;

            //Convert the raw value to the humidity in %
            double humidity = BME280_compensate_H_double(h);

            //Return the humidity as a float value
            return (float)humidity;
        }
    }
}