using System;
using System.Linq;
using Common.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public record ClassFromReflection(Type Type) : MethodHolder<ClassDeclarationSyntax>(Type.Name,
        new PropertyCollection(Type),
        Type.GetMethods().Select(x => new MethodFromReflection(x)).ToList<IMethod>(),
        new Namespace(Type.Namespace),
            ReflectionUtil.IsStatic(Type) ? Modifier.Static : Modifier.None,
            ReflectionUtil.IsStatic(Type) ? Modifier.Static : Modifier.None)
    {
        public override ClassDeclarationSyntax Syntax() => ToClass();
    }
}