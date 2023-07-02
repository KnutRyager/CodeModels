using System;
using System.Collections.Generic;
using Common.Util;

namespace Common.Algorithms.Search;

/// <summary>
/// Find the closest items by binary search
/// </summary>
public static class BinarySearchClosest
{
    public static T BinarySearchFloor<T>(IList<T> items, T item) where T : IComparable<T>
    {
        var first = 0;
        var last = items.Count - 1;
        int mid;
        do
        {
            mid = first + (last - first) / 2;
            if (item.CompareTo(items[mid]) > 0)
                first = mid + 1;
            else
                last = mid - 1;
            if (items[mid].CompareTo(item) == 0)
                break;
        } while (first <= last);
        var diff = Math.Abs(items[mid].CompareTo(item));
        var diffPrevious = Math.Abs(mid == 0 ? 99999999 : items[mid - 1].CompareTo(item));
        var index = mid == 0 || diff < diffPrevious ? mid : mid - 1;
        return items[index];
    }

    public static T BinarySearchFloor<T>(IList<T> items, T item, Func<T, T, int> compare) where T : IComparable<T>
    {
        var first = 0;
        var last = items.Count - 1;
        int mid;
        do
        {
            mid = first + (last - first) / 2;
            if (compare(item, items[mid]) > 0)
                first = mid + 1;
            else
                last = mid - 1;
            if (compare(items[mid], item) == 0)
                break;
        } while (first <= last);
        var diff = Math.Abs(compare(items[mid], item));
        var diffPrevious = Math.Abs(mid == 0 ? 99999999 : compare(items[mid - 1], item));
        var index = mid == 0 || diff < diffPrevious ? mid : mid - 1;
        return items[index];
    }

    public static T BinarySearchCeil<T>(IList<T> items, T item) where T : IComparable<T>
    {
        var first = 0;
        var last = items.Count - 1;
        int mid;
        do
        {
            mid = first + (last - first) / 2;
            if (item.CompareTo(items[mid]) > 0)
                first = mid + 1;
            else
                last = mid - 1;
            if (items[mid].CompareTo(item) == 0)
                break;
        } while (first <= last);
        var diff = Math.Abs(items[mid].CompareTo(item));
        var diffNext = mid == items.Count - 1 ? 99999999 : items[mid + 1].CompareTo(item);
        var index = mid == 0 || diff < diffNext ? mid : mid + 1;
        return items[index];
    }

    public static int? BinarySearch(IList<int> items, int item) => CollectionUtil.Lookup(items, BinarySearchIndexOf(items, item));

    public static int BinarySearchIndexOf(IList<int> items, int item)
    {
        var first = 0;
        var last = items.Count - 1;
        int mid;
        do
        {
            mid = first + (last - first) / 2;
            if (item.CompareTo(items[mid]) > 0)
                first = mid + 1;
            else
                last = mid - 1;
            if (items[mid].CompareTo(item) == 0)
                break;
        } while (first <= last);
        var diff = Math.Abs(items[mid] - item);
        var diffPrevious = Math.Abs(mid == 0 ? 99999999 : items[mid - 1] - item);
        var index = mid == 0 || diff <= diffPrevious ? mid : mid - 1;
        return index;
    }

    public static T? BinarySearch<T>(IList<T> items, T item, Func<T, T, int> compare) where T : struct => CollectionUtil.Lookup(items, IndexOfClosest(items, item, compare));

    public static int IndexOfClosest<T>(IList<T> items, T item, Func<T, T, int> compare)
    {
        var first = 0;
        var last = items.Count - 1;
        int mid;
        do
        {
            mid = first + (last - first) / 2;
            if (compare(item, items[mid]) > 0)
                first = mid + 1;
            else
                last = mid - 1;
            if (compare(items[mid], item) == 0)
                break;
        } while (first <= last);
        var diff = Math.Abs(compare(items[mid], item));
        var diffPrevious = Math.Abs(mid == 0 ? 99999999 : compare(items[mid - 1], item));
        return mid == 0 || diff <= diffPrevious ? mid : mid - 1;
    }
}
