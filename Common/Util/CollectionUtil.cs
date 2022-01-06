using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Util
{
    public static class CollectionUtil
    {
        public static List<List<T>> ToList<T>(IList<IList<T>> iList) => iList.Select(x => x.ToList()).ToList();
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable) => enumerable == null || !enumerable.Any();

        public static IList<IList<T>> ToIList<T>(List<List<T>> list)
        {
            var iList = new List<IList<T>>();
            foreach (var value in list) iList.Add(value);
            return iList;
        }
        public static List<T> MakeList<T>(Func<int, T> func, int start, int end) => Enumerable.Range(start, end - start).Select(func).ToList();
        public static List<T> MakeList<T>(T obj) => MakeList(x => obj, 0, 0);
        public static T[] L<T>(params T[] items) => items;
        public static T[] L<T>(IEnumerable<T> list, params T[] items) => list.Concat(items).ToArray();
        public static T[] LL<T>(params IEnumerable<T>[] lists) => lists.SelectMany(x => x).ToArray();

        public static List<T> Add<T>(IEnumerable<T> o, T item)
        {
            var list = o.ToList();
            list.Add(item);
            return list;
        }
        public static List<T> Add<T>(IEnumerable<T> o1, IEnumerable<T> o2)
        {
            var list = o1.ToList();
            list.AddRange(o2);
            return list;
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> toAdd) => toAdd.E_ForEach(x => collection.Add(x));

        public static List<T> Concat<T>(params IEnumerable<T>?[] lists)
        {
            var allList = new List<T>(lists.Sum(x => x?.Count() ?? 0));
            foreach (var list in lists.ToList())
            {
                if (list != null) allList = Add(allList, list);
            }
            return allList;
        }

        public static List<T>? CombineFilters<T>(params IEnumerable<T>?[] lists)
        {
            var nonEmpty = lists.Where(x => x != null).ToArray();
            if (nonEmpty.Length == 0) return null;
            var result = new List<T>(nonEmpty.Length == 0 ? 0 : nonEmpty.Max(x => x!.Count()));
            var values = Concat(nonEmpty).Distinct();
            foreach (var value in values)
            {
                if (nonEmpty.All(x => x!.Contains(value))) result.Add(value);
            }
            return result;
        }

        public static List<T> EnumeratorToList<T>(IEnumerator<T> enumerator)
        {
            var list = new List<T>();
            while (enumerator.MoveNext()) list.Add(enumerator.Current);
            return list;
        }

        public static IEnumerable<T[]> SelectRows<T>(IEnumerable<IEnumerable<T>> values)
            => values.Select(x => x.ToArray()).ToList().Skip(1);

        public static List<List<T>> Transpose<T>(IList<List<T>> lists)
        {
            var longest = lists.Any() ? lists.Max(l => l.Count) : 0;
            List<List<T>> outer = new(longest);
            for (int i = 0; i < longest; i++)
                outer.Add(new List<T>(lists.Count));
            for (int j = 0; j < lists.Count; j++)
                for (int i = 0; i < longest; i++)
                    outer[i].Add(lists[j].Count > i ? lists[j][i] : default!);
            return outer;
        }

        public static List<List<T>> RemoveColumnsAtIndices<T>(List<List<T>> list, IList<int> indices)
        {
            indices = indices.OrderByDescending(x => x).ToArray();
            foreach (var index in indices)
            {
                list.ForEach(x => x.RemoveAt(index));
            }
            return list;
        }

        public static List<List<T>> OperationAtIndex<T>(List<List<T>> list, IList<int> indices, Func<T, T> operation)
        {
            indices = indices.OrderByDescending(x => x).ToArray();
            foreach (var index in indices)
            {
                list.ForEach(x => x[index] = operation(x[index]));
            }
            return list;
        }

        public static TValue GetOrInsert<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueGenerator) where TKey : notnull
        {
            var containsKey = dictionary.ContainsKey(key);
            var value = containsKey ? dictionary[key] : valueGenerator();
            if (!containsKey) dictionary[key] = value;
            return value;
        }

        public static TValue GetOrInsertDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TKey : notnull where TValue : new()
            => GetOrInsert(dictionary, key, () => new TValue());

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider) where TKey : notnull
            => dictionary.TryGetValue(key, out TValue? value) ? value : defaultValueProvider();
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default!) where TKey : notnull
            => dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;

        public static T? Lookup<T>(IList<T> list, int index) where T : struct => index < 0 || index >= list.Count ? (T?)null : list[index];
        public static IList<int> Bucketify<T, TProp>(IList<TProp> buckets, IEnumerable<T> items, Func<T, TProp> property) where TProp : struct, IComparable<TProp>
            => Bucketify(items.Select(property).ToArray(), buckets);
        /// <summary>
        /// Bucketify by floor values
        /// </summary>
        public static IList<int> Bucketify<T>(IList<T> buckets, IEnumerable<T> items) where T : struct, IComparable<T> => items.Select(x => GetBucketIndex(buckets, x)).ToArray();
        public static int GetBucketIndex<T>(IList<T> buckets, T item) where T : struct, IComparable<T>
        {
            for (var i = 0; i < buckets.Count; i++)
            {
                if (buckets[i].CompareTo(item) > 0) return i - 1;
            }
            return buckets.Count - 1;
        }

        public static Dictionary<TKey, TValue> AddMissingEntries<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<TKey> allKeys, Func<TValue> valueGenerator) where TKey : notnull
        {
            foreach (var key in allKeys)
            {
                if (!dictionary.ContainsKey(key)) dictionary[key] = valueGenerator();
            }
            return dictionary;
        }

        public static bool IsAny<T>(this IEnumerable<T> data) => data != null && data.Any();

        public static void E_ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration) action(item);
        }

        public static int IndexOfOneBased<T>(this T[] array, T element) => Array.IndexOf(array, element) + 1;

        public static void Deconstruct<T>(this IList<T> list, out T first, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : throw new ArgumentException("No first element");
            rest = list.Skip(1).ToList();
        }

        public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out IList<T> rest)
        {
            if (list.Count < 2) throw new ArgumentException($"List too short, only length '{list.Count}'");
            first = list[0];
            second = list[1];
            rest = list.Skip(2).ToList();
        }

        public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out T third, out IList<T> rest)
        {
            if (list.Count < 3) throw new ArgumentException($"List too short, only length '{list.Count}'");
            first = list[0];
            second = list[1];
            third = list[2];
            rest = list.Skip(3).ToList();
        }

        public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out T third, out T fourth, out IList<T> rest)
        {
            if (list.Count < 4) throw new ArgumentException($"List too short, only length '{list.Count}'");
            first = list[0];
            second = list[1];
            third = list[2];
            fourth = list[3];
            rest = list.Skip(4).ToList();
        }

        public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out T third, out T fourth, out T fifth, out IList<T> rest)
        {
            if (list.Count < 5) throw new ArgumentException($"List too short, only length '{list.Count}'");
            first = list[0];
            second = list[1];
            third = list[2];
            fourth = list[3];
            fifth = list[4];
            rest = list.Skip(5).ToList();
        }

        public static List<T> OrderByPredicate<T>(this IEnumerable<T> list, params Func<T, bool>[] order) => list.Take(order.Length).Select((_, index) => list.First(order[index])).ToList();

        public static IEnumerable<T>? OnlyIfNoNulls<T>(IEnumerable<T>? values) where T : class => (values?.All(x => x != default) ?? false) ? values : default;
    }
}