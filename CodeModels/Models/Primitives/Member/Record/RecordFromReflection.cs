using System;
using System.Linq;
using CodeModels.AbstractCodeModels;
using CodeModels.AbstractCodeModels.Member;
using CodeModels.Factory;
using Common.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.AbstractCodeModelFactory;

namespace CodeModels.Models.Primitives.Member;

public record RecordFromReflection(Type ReflectedType) : AbstractTypeDeclaration<RecordDeclaration, RecordDeclarationSyntax>(ReflectedType.Name,
     NamedValues(ReflectedType),
    ReflectedType.GetMethods().Select(x => CodeModelsFromReflection.Method(x)).ToList<IMethod>(),
    new Namespace(ReflectedType.Namespace),
        ReflectionUtil.IsStatic(ReflectedType) ? Modifier.Static : Modifier.None,
        ReflectionUtil.IsStatic(ReflectedType) ? Modifier.Static : Modifier.None)
{
    public override IInstantiatedObject CreateInstance()
    {
        throw new NotImplementedException();
    }

    protected override RecordDeclaration OnToCodeModel(IAbstractCodeModelSettings? settings = null)
        => CodeModelFactory.Record(Name, null, null, null, Members());
}
