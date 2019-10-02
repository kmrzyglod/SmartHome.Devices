namespace EspIot.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return str == null || str == string.Empty;
        }
    }
}