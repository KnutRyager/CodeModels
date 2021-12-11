using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Common.DataStructures
{
    public class LazyLoadList<T> : IList<T>
    {
        private List<T>? _list;
        private readonly Func<IEnumerable<T>> _listGen;
        public List<T> List => _list ??= _listGen().ToList();

        public LazyLoadList(Func<IEnumerable<T>> listGen)
        {
            _listGen = listGen;
        }

        public T this[int index] { get => List[index]; set => List[index] = value; }
        public int Count => List.Count;
        public bool IsReadOnly => false;
        public void Add(T item) => List.Add(item);
        public void Clear() => List.Clear();
        public bool Contains(T item) => List.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => List.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => List.GetEnumerator();
        public int IndexOf(T item) => List.IndexOf(item);
        public void Insert(int index, T item) => List.Insert(index, item);
        public bool Remove(T item) => List.Remove(item);
        public void RemoveAt(int index) => List.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();
    }
}