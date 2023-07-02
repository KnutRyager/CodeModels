using System.Collections.Generic;
using System.Linq;

namespace Common.DataStructures;

public record Node<T>(T Value, List<Node<T>> Children);

public static class NodeFactory
{
    public static Node<T> Create<T>(T value, IEnumerable<Node<T>>? children = null)
        => new(value, children?.ToList() ?? new List<Node<T>>());

    public static Node<T> Create<T>(T value, params Node<T>[] children)
        => new(value, children.ToList());
}