using System;
using Common.Reflection;

namespace Common.Util
{
    public static class StringUtil
    {
        private static readonly char[] numberCharacters = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        public static string GetInside(string str, string start, string end) => str[(str.IndexOf(start) + start.Length)..str.IndexOf(end)];

        public static int GetIndexOfFirstNumber(string str) => str.IndexOfAny(numberCharacters);

        public static T? Parse<T>(string? str) where T : struct => typeof(T) switch
        {
            _ when str == null => null,
            var type when type == typeof(string) => ReflectionUtil.ConvertTo<T>(str),
            var type when type == typeof(int) => ReflectionUtil.ConvertTo<T>(str),
            var type when type == typeof(double) => ReflectionUtil.ConvertTo<T>(str),
            var type when type == typeof(decimal) => ReflectionUtil.ConvertTo<T>(str),
            var type when type == typeof(DateTime) => ReflectionUtil.ConvertTo<T>(str),
            //var type when type == typeof(string) => str,
            //var type when type == typeof(int) => int.Parse(str),
            //var type when type == typeof(double) => double.Parse(str),
            //var type when type == typeof(decimal) => decimal.Parse(str),
            //var type when type == typeof(DateTime) => DateTime.Parse(str),
            _ => throw new ArgumentException($"Unhandled property type: '{typeof(T).Name}'")
        };

        public static string CleanCodeString(string s) => s
            .Replace("\\", "\\\\")
            .Replace("\"", "\"\"");

        public static string FilterJoin(string? str1, string? str2, string separator = ".") => $"{str1}{((string.IsNullOrWhiteSpace(str1)|| string.IsNullOrWhiteSpace(str2)) ? string.Empty : separator)}{str2}";
    }
}