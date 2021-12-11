using System;
using System.Collections.Generic;
using Common.Files;
using Common.Util;

namespace Common.Extensions
{
    public static class Extensions
    {
        public static string ToJson<T>(this T obj) => obj == null ? "" : JsonUtil.ToJson(obj);

        public static void WriteToFile(this string str, string path) => FileUtil.WriteTextToFile(path, str);

        public static int BinarySearch<T>(this List<T> list, T item, Func<T?, T?, int> compare)
            => list.BinarySearch(item, new ComparisonComparer<T>(compare));

        public static string[] Split(this string str, string separator)
            => str.Split(new[] { separator }, str.Length, StringSplitOptions.None);

        public class ComparisonComparer<T> : IComparer<T>
        {
            private readonly Comparison<T?> comparison;

            public ComparisonComparer(Func<T?, T?, int> compare)
            {
                comparison = new(compare);
            }

            public int Compare(T? x, T? y) => comparison(x, y);
        }
    }
}