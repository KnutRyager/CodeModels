using System;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions;

namespace Common.DataStructures;

public interface IMappingTable<TSource, TTarget>
{
    TTarget this[TSource value] { get; }
}

public class TupleMappingTable<TIndex, TTuple> : IMappingTable<TIndex, TTuple> where TIndex : IComparable<TIndex>
{
    public List<(TIndex, TTuple)> Rows { get; }

    public TupleMappingTable(IEnumerable<(TIndex, TTuple)> rows)
    {
        Rows = rows.ToList();
        Rows.Sort((x, y) => x.Item1.CompareTo(y.Item1));
    }

    public TTuple this[TIndex value] => Rows[Rows!.BinarySearch((value, default), (x, y) => x.Item1.CompareTo(y.Item1))].Item2;
}

public class MappingTable<T1, T2> : TupleMappingTable<T1, (T1, T2)> where T1 : IComparable<T1>
{
    public MappingTable(IEnumerable<(T1, T2)> rows) : base(rows.Select(x => (x.Item1, x))) { }
}

public class MappingTable<T1, T2, T3> : TupleMappingTable<T1, (T1, T2, T3)> where T1 : IComparable<T1>
{
    public MappingTable(IEnumerable<(T1, T2, T3)> rows) : base(rows.Select(x => (x.Item1, x))) { }
}

public class MappingTable<T1, T2, T3, T4> : TupleMappingTable<T1, (T1, T2, T3, T4)> where T1 : IComparable<T1>
{
    public MappingTable(IEnumerable<(T1, T2, T3, T4)> rows) : base(rows.Select(x => (x.Item1, x))) { }
}
