using System;

namespace Common.DataStructures
{
    public interface ITraversableTree<T>
    {
        T Data { get; }
        ITraversableTree<T> this[int i] { get; }
        void Traverse(ITraversableTree<T> node, Action<T> visitor);
    }

    /// <summary>
    /// Tree data structure.
    /// </summary>
    public interface ITraversableTree<T, TTreeType> where TTreeType : ITraversableTree<T, TTreeType>
    {
        T Data { get; }
        TTreeType this[int i] { get; }
        void Traverse(TTreeType node, Action<T> visitor);
    }


    /// <summary>
    /// Tree data structure.
    /// </summary>
    public interface IWriteableTree<T>
    {
        void AddChild(T data);
    }

    /// <summary>
    /// Tree data structure.
    /// </summary>
    public interface ITree<T> : ITraversableTree<T>, IWriteableTree<T> { }

    /// <summary>
    /// Tree data structure.
    /// </summary>
    public interface ITree<T, TTreeType> : ITraversableTree<T, TTreeType>, IWriteableTree<T> where TTreeType : ITraversableTree<T, TTreeType> { }
}