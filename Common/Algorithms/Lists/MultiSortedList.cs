using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Common.DataStructures;

/// <summary>
/// A list that will remain sorted based on a list of sort properties.
/// </summary>
public class MultiSortedList<T> : IEnumerable<T>
{
    private List<T> _list = new();
    private readonly Func<T, object>? _orderBy;
    private readonly List<Func<T, object>> _thenBys;
    private bool _isSorted = true;

    public MultiSortedList(params Func<T, object>[] sortings)
    {
        var sortingsList = sortings == null ? new List<Func<T, object>>() : sortings.ToList();
        _orderBy = sortingsList.Count == 0 ? null : sortingsList[0];
        _thenBys = sortingsList.GetRange(1, sortingsList.Count - 1);
    }

    public void Add(T obj)
    {
        _list.Add(obj);
        _isSorted = false;
    }

    public void Add(IEnumerable<T> objs)
    {
        foreach (var obj in objs) Add(obj);
    }

    public T this[int i]
    {
        get
        {
            Sort();
            return _list[i];
        }
        set
        {
            Sort();
            _list[i] = value;
            _isSorted = false;
        }
    }

    public int Count => _list.Count;
    public bool TrueForAll(Predicate<T> condition) => _list.TrueForAll(condition);

    private void Sort()
    {
        if (_isSorted || _orderBy == null) return;
        var ordered = _list.OrderBy(_orderBy);
        foreach (var thenBy in _thenBys)
        {
            ordered = ordered.ThenBy(thenBy);
        }
        _list = ordered.ToList();
        _isSorted = true;
    }

    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
