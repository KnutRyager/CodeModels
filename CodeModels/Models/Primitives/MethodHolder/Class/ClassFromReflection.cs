using System;
using System.Linq;
using CodeModels.Factory;
using Common.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.AbstractCodeModelFactory;

namespace CodeModels.Models;

public record ClassFromReflection(Type ReflectedType) : TypeDeclaration<ClassDeclarationSyntax>(ReflectedType.Name,
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

    public override ClassDeclarationSyntax Syntax() => ToClass();
}
