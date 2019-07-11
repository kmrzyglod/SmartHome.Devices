﻿// Source code is modified from Mike Jones's JSON Serialization and Deserialization library (https://www.ghielectronics.com/community/codeshare/entry/357)

namespace Json.NetMF

{
    internal static class StringExtensions
    {
        public static bool EndsWith(this string s, string value)
        {
            return s.IndexOf(value) == s.Length - value.Length;
        }

        public static bool StartsWith(this string s, string value)
        {
            return s.IndexOf(value) == 0;
        }

        public static bool Contains(this string s, string value)
        {
            return s.IndexOf(value) >= 0;
        }
    }
}