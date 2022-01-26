using System;
using System.Collections.Generic;
using Common.DataStructures;
using Common.Files;
using Common.Util;

namespace System.Linq;

public static class LinqExtensions
{
    public static EqualityList<T> ToEqualityList<T>(this IEnumerable<T> enumerable) => enumerable == null ? throw new ArgumentException(" enumerable is null")
        : new EqualityList<T>(enumerable);
}
