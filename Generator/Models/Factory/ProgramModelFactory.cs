using System.Collections.Generic;
using CodeAnalyzation.Models.ProgramModels;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public static class ProgramModelFactory
{
    public static ModelRegistry ModelRegistry(IEnumerable<PropertyCollection> Models, 
        IEnumerable<InstanceClass>? Singletons = null)
        => new(List(Models), List(Singletons));

    public static IType Type(ModelModel Model) => new QuickType(Model.Name);
}
