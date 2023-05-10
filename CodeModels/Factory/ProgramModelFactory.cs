using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.AbstractCodeModels.Member;
using CodeModels.Models;
using CodeModels.ProgramModels;
using CodeModels.ProgramModels.ModelModel;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Factory;

public static class ProgramModelFactory
{
    public static ModelRegistry ModelRegistry(IEnumerable<NamedValueCollection> Models,
        IEnumerable<InstanceClass>? Singletons = null)
        => new(List(Models), List(Singletons));

    public static IType Type(ModelModel Model) => QuickType(Model.Name);
}
