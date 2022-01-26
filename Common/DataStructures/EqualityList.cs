using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Common.DataStructures;

/// <summary>
/// A list with equality check for actually comparing content.
/// </summary>
public class EqualityList<T> : IList<T>
{
    private readonly List<T> _list;

    public EqualityList() { _list = new List<T>(); }
    public EqualityList(params T[] objects) : this() { _list.AddRange(objects); }
    public EqualityList(IEnumerable<T> objects) : this() { _list.AddRange(objects); }
    public EqualityList(List<T> objects) { _list = objects; }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        var otherList = obj as EqualityList<T>;
        if ((otherList is null) || otherList.Count != Count) return false;
        for (var i = 0; i < _list.Count; i++)
        {
            if (!_list[i]?.Equals(otherList[i]) ?? otherList[i] != null) return false;
        }
        return true;
    }

    public override string ToString() => $"[{string.Join(", ", _list.Select(x => x?.ToString()))}]";

    public override int GetHashCode()
    {
        var hashCode = 1430287;
        for (var i = 0; i < _list.Count; i++)
        {
            hashCode = hashCode * 7302013 ^ _list[i]?.GetHashCode() ?? 0;
        }
        return hashCode;
    }

    public T this[int index] { get => _list[index]; set => _list[index] = value; }
    public int Count => _list.Count;
    public bool IsReadOnly => false;
    public void Add(T item) => _list.Add(item);
    public void Clear() => _list.Clear();
    public bool Contains(T item) => _list.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    public int IndexOf(T item) => _list.IndexOf(item);
    public void Insert(int index, T item) => _list.Insert(index, item);
    public bool Remove(T item) => _list.Remove(item);
    public void RemoveAt(int index) => _list.RemoveAt(index);
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
}
