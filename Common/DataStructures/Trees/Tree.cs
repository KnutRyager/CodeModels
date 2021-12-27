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
        public Tree<T>? Parent { get; private set; }
        private readonly List<Tree<T>> _children;

        public Tree(T data, IEnumerable<Tree<T>> children)
        {
            Data = data;
            _children = new(children ?? Enumerable.Empty<Tree<T>>());
            foreach (var child in _children) child.Parent = this;
        }

        public Tree(params Tree<T>[] valueAndChldren) : this(valueAndChldren[0], valueAndChldren.Skip(1)) { }
        public Tree(IEnumerable<T> valueAndChildren) : this(valueAndChildren.First(), valueAndChildren.Skip(1).Select(x => new Tree<T>(x))) { }
        public Tree(T data) : this(data, Array.Empty<Tree<T>>()) { }
        public Tree(Tree<T> tree)
        {
            Data = tree.Data;
            _children = tree._children.Select(x => new Tree<T>(x)).ToList();
            foreach (var child in _children) child.Parent = this;
        }

        public Tree<T> this[int i] => _children[i];

        public void Traverse(Tree<T> node, Action<T> visitor)
        {
            visitor(node.Data);
            foreach (var child in node._children) Traverse(child, visitor);
        }

        public void Traverse(Action<T> visitor) => Traverse(this, visitor);

        public void TraverseNodes(Tree<T> node, Action<Tree<T>> visitor)
        {
            visitor(node);
            foreach (var child in node._children) TraverseNodes(child, visitor);
        }

        public void TraverseNodes(Action<Tree<T>> visitor) => TraverseNodes(this, visitor);

        public Tree<T> Add(T data)
        {
            var node = new Tree<T>(data);
            _children.Add(node);
            return node;
        }

        public void AddRange(IEnumerable<T> data)
        {
            foreach (var item in data) Add(item);
        }

        public Tree<T> Add(Tree<T> node)
        {
            _children.Add(node);
            return node;
        }

        public void AddRange(IEnumerable<Tree<T>> data)
        {
            foreach (var item in data) Add(item);
        }

        public Tree<T2> Transform<T2>(Func<T, T2> transformer) => new(transformer(Data), _children.Select(x => x.Transform(transformer)));
        public Tree<T2> Transform<T2>(Func<T, IEnumerable<T>, T2> transformer) => new(transformer(Data, _children.Select(x => x.Data)), _children.Select(x => x.Transform(transformer)));
        public T2 To<T2>(Func<T, IEnumerable<T2>, T2> transformer) => transformer(Data, _children.Select(x => x.To(transformer)));
        public Tree<T2> TransformByParent<T2>(Func<T, T?, T2> transformer)
        {
            var result = transformer(Data, default);
            var tree = new Tree<T2>(result);
            TransformByParentCollect(tree, transformer);
            return tree;
        }

        private void TransformByParentCollect<T2>(Tree<T2> tree, Func<T, T?, T2> transformer)
        {
            foreach (var child in _children)
            {
                var result = transformer(child, this);
                child.TransformByParentCollect(tree.Add(result), transformer);
            }
        }

        public Tree<T2> TransformByTransformedParent<T2>(Func<T, T2?, T2> transformer)
        {
            var result = transformer(Data, default);
            var tree = new Tree<T2>(result);
            TransformByTransformedParentCollect(tree, transformer);
            return tree;
        }

        private void TransformByTransformedParentCollect<T2>(Tree<T2> tree, Func<T, T2?, T2> transformer)
        {
            foreach (var child in _children)
            {
                var result = transformer(child, tree);
                child.TransformByTransformedParentCollect(tree.Add(result), transformer);
            }
        }

        public List<T> ToList()
        {
            var result = new List<T>();
            Traverse(this, x => result.Add(x));
            return result;
        }

        public static implicit operator T(Tree<T> tree) => tree.Data;
        public static implicit operator Tree<T>(T data) => new(data);
    }
}