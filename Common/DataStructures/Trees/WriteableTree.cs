using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.DataStructures
{
    /// <summary>
    /// Tree with linked list node list.
    /// </summary>
    public class WriteableTree<T> : ITree<T, WriteableTree<T>>
    {
        public T Data { get; set; }
        private readonly LinkedList<WriteableTree<T>> _children;

        public WriteableTree(T data, IEnumerable<WriteableTree<T>>? children = null)
        {
            Data = data;
            _children = new(children ?? Enumerable.Empty<WriteableTree<T>>());
        }

        public void AddChild(T data) => _children.AddFirst(new WriteableTree<T>(data));

        public WriteableTree<T> this[int i]
        {
            get
            {
                foreach (var child in _children) if (--i == 0) return child;
                throw new ArgumentException($"Index out of range: '{i}'.");
            }
        }

        public void Traverse(WriteableTree<T> node, Action<T> visitor)
        {
            visitor(node.Data);
            foreach (var child in node._children) Traverse(child, visitor);
        }
    }
}