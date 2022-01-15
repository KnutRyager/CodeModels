using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CodeAnalyzation.Models;
using Common.Reflection;
using Common.Util;
using Models;

namespace CodeAnalyzation.DataTransformation;

[Model]
public class Assembly : IAssembly<Assembly, Namespace>
{
    [Key] public int Id { get; set; }
    public string Name { get; set; } = default!;
    //public string? Version { get; set; } = default!;
    public bool AnalysisPerformed { get; set; }
    public DateTime? AnalysisTime { get; set; }
    public System.Reflection.Assembly? _assembly;
    public System.Reflection.Assembly AssemblyRef => _assembly ??= ReflectionSerialization.DeserializeAssembly(Name);
    public virtual ICollection<Namespace> Namespaces { get; set; } = default!;
    public virtual ICollection<AssemblyDependency> DependencyRelations { get; set; } = default!;
    public virtual ICollection<AssemblyDependency> DependantRelations { get; set; } = default!;
    [NotMapped] private ICollection<Assembly>? _dependencies;
    [NotMapped] public ICollection<Assembly> Dependencies => _dependencies ??= CollectionUtil.OnlyIfNoNulls(DependencyRelations?.Select(x => x.Dependency))?.ToList()!;
    [NotMapped] private ICollection<Assembly>? _directDependencies;
    [NotMapped] public ICollection<Assembly> DirectDependencies => _directDependencies ??= CollectionUtil.OnlyIfNoNulls(DependencyRelations?.Where(x => x.IsDirect).Select(x => x.Dependency))?.ToList()!;
    [NotMapped] private ICollection<Assembly>? _directDependendants;
    [NotMapped] public ICollection<Assembly> DirectDependants => _directDependendants ??= CollectionUtil.OnlyIfNoNulls(DependantRelations?.Where(x => x.IsDirect).Select(x => x.Dependant))?.ToList()!;
    [NotMapped] private ICollection<Assembly>? _dependants;
    [NotMapped] public ICollection<Assembly> Dependants => _dependants ??= CollectionUtil.OnlyIfNoNulls(DependantRelations?.Select(x => x.Dependant))?.ToList()!;
    public string Hash { get; private set; } = default!;
    public string SimpleName => Name.Split(',')[0];

    ICollection<INamespace> IAssembly<Assembly, Namespace>.Namespaces => throw new NotImplementedException();

    public string? Version => throw new NotImplementedException();

    ICollection<INamespace> IAssembly.Namespaces => (ICollection<INamespace>)Namespaces;
    ICollection<IAssembly> IAssembly.Dependencies => (ICollection<IAssembly>)Dependencies;
    ICollection<IAssembly> IAssembly.DirectDependencies => (ICollection<IAssembly>)DirectDependencies;
    ICollection<IAssembly> IAssembly.DirectDependants => (ICollection<IAssembly>)DirectDependants;
    ICollection<IAssembly> IAssembly.Dependants => (ICollection<IAssembly>)Dependants;
}
