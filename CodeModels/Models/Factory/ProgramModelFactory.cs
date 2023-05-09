using System.Collections.Generic;
using CodeModels.Models.ProgramModels;
using static CodeModels.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public static class ProgramModelFactory
{
    public static ModelRegistry ModelRegistry(IEnumerable<PropertyCollection> Models,
        IEnumerable<InstanceClass>? Singletons = null)
        => new(List(Models), List(Singletons));

    public static IType Type(ModelModel Model) => CodeModelFactory.QuickType(Model.Name);
}
