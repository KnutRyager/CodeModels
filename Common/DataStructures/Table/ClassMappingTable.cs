using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Algorithms.Search;
using Common.Extensions;

namespace Common.DataStructures;

public class ClassMappingTable<TIndex, Tentry> : ILookupTable<TIndex, Tentry>
    where TIndex : IComparable<TIndex>
{
    public int Count => Rows.Count;
    public IEnumerator<Tentry> GetEnumerator() => Rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Rows.GetEnumerator();
    public IList<Tentry> Rows { get; }    // Warning: Rows not sorted. Don't mix with RowsFull.
    private readonly Func<Tentry, TIndex> _selector;
    protected List<(TIndex, Tentry)>? _rowsFullInited;
    public List<(TIndex, Tentry)> RowsFull => _rowsFullInited ??= Init();

    public ClassMappingTable(IList<Tentry> rows, Func<Tentry, TIndex> selector)
    {
        Rows = rows;
        _selector = selector;
    }

    private List<(TIndex, Tentry)> Init()
    {
        _rowsFullInited = Rows.Select(x => (_selector(x), x)).ToList();
        _rowsFullInited.Sort((x, y) => x.Item1.CompareTo(y.Item1));
        return _rowsFullInited;
    }

    public bool Contains(TIndex value) => IndexOf(value) >= 0;
    public Tentry MaybeValue(TIndex value) => Contains(value) ? this[value] : default!;
    public Tentry GetEntry(int index) => RowsFull[index].Item2;
    public Tentry this[TIndex value] => GetEntry(IndexOf(value));
    public int IndexOf(TIndex value) => RowsFull!.BinarySearch((value, default), (x, y) => x.Item1.CompareTo(y.Item1));
    public int IndexOfClosest(TIndex value) => BinarySearchClosest.IndexOfClosest(RowsFull!, (value, default), (x, y) => x.Item1.CompareTo(y.Item1));
    public (TIndex, Tentry)? Closest(TIndex value) => BinarySearchClosest.BinarySearch(RowsFull!, (value, default), (x, y) => x.Item1.CompareTo(y.Item1))!;
    public Tentry ClosestFloor(TIndex value) => BinarySearchClosest.BinarySearchFloor(RowsFull!, (value, default), (x, y) => x.Item1.CompareTo(y.Item1)).Item2!;

    public Tentry GetEntry(TIndex Entry, int offset = 0)
    {
        throw new NotImplementedException();
    }
}
