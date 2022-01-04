using System;
using System.Collections.Generic;

namespace CodeAnalyzation.Models
{
    public interface INamespace
    {
        string Name { get; }
        string FullName { get; }
        int Depth { get; set; }
        bool IsGenericTypeDefinition { get; set; }
        bool IsConstructedGenericType { get; set; }
        INamespace? ParentNamespace { get; set; }
        INamespace? GenericDefinitionNamespace { get; set; }
        IAssembly? Assembly { get; set; }
        ICollection<INamespace> ChildrenNamespaces { get; }
        ICollection<INamespace> GenericInstanceNamespaces { get; }
        //ICollection<NamespaceContainment> AllParentNamespaces { get; }
        //ICollection<NamespaceContainment> AllChildNamespaces { get; }
        ICollection<IDataType> DataTypes { get; }
        ICollection<IOperation> Operations { get; }
        string? PrimitiveTypeSerialized { get; }
        IList<INamespace> ParentNamespaces { get; }
        IList<INamespace> ChildNamespaces { get; }
        IList<INamespace> DirectChildNamespaces { get; }
        Type? Type { get; }
        bool IsType { get; }
    }

    interface INamespace<TAssembly, TNamespace, TOperation, TDataType> : INamespace
       where TAssembly : IAssembly
       where TNamespace : INamespace
       where TOperation : IOperation
       where TDataType : IDataType
    {
        new TNamespace? ParentNamespace { get; set; }
        new TNamespace? GenericDefinitionNamespace { get; set; }
        new TAssembly? Assembly { get; set; }
        new ICollection<TNamespace> ChildrenNamespaces { get; }
        new ICollection<TNamespace> GenericInstanceNamespaces { get; }
        //new ICollection<TNamespaceContainment> AllParentNamespaces { get; }
        //new ICollection<TNamespaceContainment> AllChildNamespaces { get; }
        new ICollection<TDataType> DataTypes { get; }
        new ICollection<TOperation> Operations { get; }
        new IList<TNamespace> ParentNamespaces { get; }
        new IList<TNamespace> ChildNamespaces { get; }
        new IList<TNamespace> DirectChildNamespaces { get; }
    }
}