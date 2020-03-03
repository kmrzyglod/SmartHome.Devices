using System;

namespace EspIot.Core.Helpers
{
    public static class Logger
    {
        public delegate string LogFunction();

        public enum LogLevel
        {
            Info,
            Warning,
            Error
        }

        public static void Log(string message, LogLevel logLevel = LogLevel.Info)
        {
#if DEBUG
            Console.WriteLine($"{LogLevelToString(logLevel)}: {message}");
#endif
            //TODO save all logs on micro SD card if available in device
        }

        //Use when evaluating message to log is expensive (e.g. some serialization is needed) and lazy evaluation is more efficient
        public static void Log(LogFunction logFunction, LogLevel logLevel = LogLevel.Info)
        {
#if DEBUG
            Console.WriteLine($"{LogLevelToString(logLevel)}: {logFunction.Invoke()}");
#endif
            //TODO save all logs on micro SD card if available in device
        }

        private static string LogLevelToString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Info:
                    return "Info";
                case LogLevel.Warning:
                    return "Warning";
                case LogLevel.Error:
                    return "Error";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }
    }
}