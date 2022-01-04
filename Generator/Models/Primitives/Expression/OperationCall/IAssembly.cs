using System.Collections.Generic;

namespace CodeAnalyzation.Models
{
    public interface IAssembly
    {
        string Name { get; }
        string? Version { get; }
        System.Reflection.Assembly AssemblyRef { get; }
        ICollection<INamespace> Namespaces { get; }
        //ICollection<AssemblyDependency> DependencyRelations { get; }
        //ICollection<AssemblyDependency> DependantRelations { get; }
        ICollection<IAssembly> Dependencies { get; }
        ICollection<IAssembly> DirectDependencies { get; }
        ICollection<IAssembly> DirectDependants { get; }
        ICollection<IAssembly> Dependants { get; }
        string SimpleName { get; }
    }

    public interface IAssembly<TAssembly, TNamespace> : IAssembly
        where TAssembly : IAssembly
        where TNamespace : INamespace
    {
        new ICollection<INamespace> Namespaces { get; }
        //new ICollection<AssemblyDependency> DependencyRelations { get; }
        //new ICollection<AssemblyDependency> DependantRelations { get; }
        new ICollection<TAssembly> Dependencies { get; }
        new ICollection<TAssembly> DirectDependencies { get; }
        new ICollection<TAssembly> DirectDependants { get; }
        new ICollection<TAssembly> Dependants { get; }
    }
}