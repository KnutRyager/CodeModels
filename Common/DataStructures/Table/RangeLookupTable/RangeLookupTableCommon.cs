using System;
using System.Collections.Generic;

namespace Common.DataStructures;

public enum LookupRangeTableMode
{
    Exact, Floor, Ceiling
}

public static class RangeLookupTableCommon
{
    public static int Compare<T>(LookupRangeTableMode mode, T element, IList<T> list, int i) where T : IComparable<T>
    {
        var comparison = element.CompareTo(list[i]);
        return mode switch
        {
            LookupRangeTableMode.Exact => comparison,
            LookupRangeTableMode.Floor => (i == list.Count - 1 || element.CompareTo(list[i + 1]) < 0) ? (comparison >= 0 ? 0 : comparison) : comparison,
            LookupRangeTableMode.Ceiling => (i == 0 || element.CompareTo(list[i - 1]) > 0) ? (comparison <= 0 ? 0 : comparison) : comparison,
            _ => throw new NotImplementedException($"Unhanded: {mode}"),
        };
    }

    public static int RangeBinarySearchIndexOf<T>(LookupRangeTableMode mode, IList<T> list, T value) where T : IComparable<T>
    {
        var lower = 0;
        var upper = list.Count - 1;

        while (lower <= upper)
        {
            var middle = lower + (upper - lower) / 2;
            var comparisonResult = Compare(mode, value, list, middle);
            if (comparisonResult == 0)
                return middle;
            else if (comparisonResult < 0)
                upper = middle - 1;
            else
                lower = middle + 1;
        }

        return ~lower;
    }
}
