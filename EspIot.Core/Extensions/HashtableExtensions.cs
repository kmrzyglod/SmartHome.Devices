using System;
using System.Collections;

namespace EspIot.Core.Extensions
{
    public static class HashtableExtensions
    {
        //Try get int value from hashtable or get default if value isn't int
        public static int GetInt(this Hashtable hashtable, string key)
        {
            return hashtable[key.ToLower()] is int value ? value : default;
        }

        //Try get uint value from hashtable or get default if value isn't uint
        public static uint GetUInt(this Hashtable hashtable, string key)
        {
            return hashtable[key.ToLower()] is uint value ? value : default;
        }

        //Try get short value from hashtable or get default if value isn't short
        public static short GetShort(this Hashtable hashtable, string key)
        {
            return hashtable[key.ToLower()] is short value ? value : default;
        }

        //Try get ushort value from hashtable or get default if value isn't ushort
        public static ushort GetUShort(this Hashtable hashtable, string key)
        {
            return hashtable[key.ToLower()] is ushort value ? value : default;
        }

        //Try get float value from hashtable or get default if value isn't float
        public static float GetFloat(this Hashtable hashtable, string key)
        {
            return hashtable[key.ToLower()] is float value ? value : default;
        }

        //Try get double value from hashtable or get default if value isn't double
        public static double GetDouble(this Hashtable hashtable, string key)
        {
            return hashtable[key.ToLower()] is double value ? value : default;
        }

        //Try get string value from hashtable or get default if value isn't string
        public static string GetString(this Hashtable hashtable, string key)
        {
            return hashtable[key.ToLower()] is string value ? value : default;
        }

        //Try get DateTime value from hashtable or get default if value isn't DateTime
        public static DateTime GetDateTime(this Hashtable hashtable, string key)
        {
            return hashtable[key.ToLower()] is DateTime value ? value : default;
        }

        //Try get DateTime value from hashtable or get default if value isn't DateTime
        public static bool GetBool(this Hashtable hashtable, string key)
        {
            return hashtable[key.ToLower()] is bool value ? value : default;
        }

        public static object[] GetList(this Hashtable hashtable, string key)
        {
            return (hashtable[key.ToLower()] as ArrayList)?.ToArray() ?? new object[] { };
        }

        public static long[] GetLongList(this Hashtable hashtable, string key)
        {
            return (hashtable[key.ToLower()] as ArrayList)?.ToArray(typeof(long)) as long[] ?? new long[] { };
        }

        public static ulong[] GetULongList(this Hashtable hashtable, string key)
        {
            var values = hashtable[key.ToLower()] as ArrayList ?? new ArrayList();
            var result = new ulong[values.Count];

            for (int i = 0; i < values.Count; i++)
            {
                result[i] = (ulong) (long) values[i];
            }

            return result;
        }

        public static int[] GetIntList(this Hashtable hashtable, string key)
        {
            var values = hashtable[key.ToLower()] as ArrayList ?? new ArrayList();
            var result = new int[values.Count];

            for (int i = 0; i < values.Count; i++)
            {
                result[i] = (int) (long) values[i];
            }

            return result;
        }

        public static uint[] GetUIntList(this Hashtable hashtable, string key)
        {
            var values = hashtable[key.ToLower()] as ArrayList ?? new ArrayList();
            var result = new uint[values.Count];

            for (int i = 0; i < values.Count; i++)
            {
                result[i] = (uint) (long) values[i];
            }

            return result;
        }

        public static short[] GetShortList(this Hashtable hashtable, string key)
        {
            var values = hashtable[key.ToLower()] as ArrayList ?? new ArrayList();
            var result = new short[values.Count];

            for (int i = 0; i < values.Count; i++)
            {
                result[i] = (short) (long) values[i];
            }

            return result;
        }

        public static ushort[] GetUShortList(this Hashtable hashtable, string key)
        {
            var values = hashtable[key.ToLower()] as ArrayList ?? new ArrayList();
            var result = new ushort[values.Count];

            for (int i = 0; i < values.Count; i++)
            {
                result[i] = (ushort) (long) values[i];
            }

            return result;
        }

        public static float[] GetFloatList(this Hashtable hashtable, string key)
        {
            var values = hashtable[key.ToLower()] as ArrayList ?? new ArrayList();
            var result = new float[values.Count];

            for (int i = 0; i < values.Count; i++)
            {
                result[i] = (float) (double) values[i];
            }

            return result;
        }

        public static double[] GetDoubleList(this Hashtable hashtable, string key)
        {
            return (hashtable[key.ToLower()] as ArrayList)?.ToArray(typeof(double)) as double[] ?? new double[] { };
        }

        public static DateTime[] GetDateTimeList(this Hashtable hashtable, string key)
        {
            return (hashtable[key.ToLower()] as ArrayList)?.ToArray(typeof(DateTime)) as DateTime[] ??
                   new DateTime[] { };
        }

        public static string[] GetStringList(this Hashtable hashtable, string key)
        {
            return (hashtable[key.ToLower()] as ArrayList)?.ToArray(typeof(string)) as string[] ?? new string[] { };
        }
    }
}