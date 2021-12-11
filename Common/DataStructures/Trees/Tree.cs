using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.DataStructures
{
    /// <summary>
    /// Tree data structure that supports transformations.
    /// </summary>
    public class Tree<T> : ITraversableTree<T, Tree<T>>
    {
        public T Data { get; }
        private readonly Tree<T>[] _children;

        public Tree(T data, IEnumerable<Tree<T>> children)
        {
            Data = data;
            _children = children.ToArray();
        }

        public Tree(params Tree<T>[] valueAndChldren) : this(valueAndChldren[0], valueAndChldren.Skip(1)) { }
        public Tree(T data) : this(data, Array.Empty<Tree<T>>()) { }

        public Tree<T> this[int i] => _children[i];

        public void Traverse(Tree<T> node, Action<T> visitor)
        {
            visitor(node.Data);
            foreach (var child in node._children) Traverse(child, visitor);
        }

        public Tree<T2> Transform<T2>(Func<T, T2> transformer) => new(transformer(Data), _children.Select(x => x.Transform(transformer)));
        public Tree<T2> Transform<T2>(Func<T, IEnumerable<T>, T2> transformer) => new(transformer(Data, _children.Select(x => x.Data)), _children.Select(x => x.Transform(transformer)));
        public T2 To<T2>(Func<T, IEnumerable<T2>, T2> transformer) => transformer(Data, _children.Select(x => x.To(transformer)));

        public static implicit operator T(Tree<T> tree) => tree.Data;
        public static implicit operator Tree<T>(T data) => new(data);
    }
}