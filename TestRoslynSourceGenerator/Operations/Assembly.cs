using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Reflection;
using Common.Util;
using Models;

namespace TheEverythingAPI.DataTransformation;

[Model]
public class Assembly
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
    [NotMapped] private List<Assembly>? _dependencies;
    [NotMapped] public List<Assembly> Dependencies => _dependencies ??= CollectionUtil.OnlyIfNoNulls(DependencyRelations?.Select(x => x.Dependency))?.ToList()!;
    [NotMapped] private List<Assembly>? _directDependencies;
    [NotMapped] public List<Assembly> DirectDependencies => _directDependencies ??= CollectionUtil.OnlyIfNoNulls(DependencyRelations?.Where(x => x.IsDirect).Select(x => x.Dependency))?.ToList()!;
    [NotMapped] private List<Assembly>? _directDependendants;
    [NotMapped] public List<Assembly> DirectDependants => _directDependendants ??= CollectionUtil.OnlyIfNoNulls(DependantRelations?.Where(x => x.IsDirect).Select(x => x.Dependant))?.ToList()!;
    [NotMapped] private List<Assembly>? _dependants;
    [NotMapped] public List<Assembly> Dependants => _dependants ??= CollectionUtil.OnlyIfNoNulls(DependantRelations?.Select(x => x.Dependant))?.ToList()!;
    public string Hash { get; private set; } = default!;
    public string SimpleName => Name.Split(",")[0];
}