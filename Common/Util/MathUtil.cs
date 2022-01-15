using System;

namespace Common.Util;

public static class MathUtil
{
    public static bool InRange<T>(T value, T minInclusive, T maxExclusive) where T : IComparable<T> => value.CompareTo(minInclusive) >= 0 && value.CompareTo(maxExclusive) < 0;
    public static int AddBoundedOffset(int value, int offset, int minInclusive, int maxExclusive)
        => Math.Min(maxExclusive, Math.Max(minInclusive, value + offset));

    public static T? Min<T>(T? lhs, T? rhs) where T : struct, IComparable<T> => lhs == null ? rhs : rhs == null ? lhs : lhs.Value.CompareTo(rhs.Value) < 0 ? lhs : rhs;
    public static T? Max<T>(T? lhs, T? rhs) where T : struct, IComparable<T> => lhs == null ? rhs : rhs == null ? lhs : lhs.Value.CompareTo(rhs.Value) >= 0 ? lhs : rhs;
    public static int? OnlyIfNonDefault(int? value) => value != 0 ? value : null;
    public static decimal? OnlyIfNonDefault(decimal? value) => value != 0 ? value : null;
    public static decimal Average(decimal sum, decimal? denominator) => (denominator ?? 0) == 0 ? 0 : sum / denominator!.Value;
}
