using System.ComponentModel.DataAnnotations;
using Models;

namespace CodeAnalyzation.DataTransformation;

[Model]
public class AssemblyDependency
{
    [Key] public int Id { get; set; }
    public int DependencyId { get; set; }
    public int DependantId { get; set; }
    public bool IsDirect { get; set; }
    public Assembly Dependency { get; set; } = default!;
    public Assembly Dependant { get; set; } = default!;

    public AssemblyDependency() { }

    public AssemblyDependency(Assembly dependency, Assembly dependant, bool isDirectDependency = false)
    {
        Dependency = dependency;
        Dependant = dependant;
        IsDirect = isDirectDependency;
    }

    public override string ToString() => $"{Dependency}=>{Dependant}";
}
