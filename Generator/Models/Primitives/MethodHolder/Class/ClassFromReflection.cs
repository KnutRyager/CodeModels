using System;
using System.Linq;
using Common.Reflection;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzation.Models
{
    public record ClassFromReflection(Type Type) : MethodHolder(Type.Name,
        new PropertyCollection(Type),
        Type.GetMethods().Select(x => new MethodFromReflection(x)).ToList<IMethod>(),
        new Namespace(Type.Namespace),
            ReflectionUtil.IsStatic(Type) ? Modifier.Static : Modifier.None,
            ReflectionUtil.IsStatic(Type) ? Modifier.Static : Modifier.None, ReflectionUtil.IsStatic(Type))
    {
        public override CSharpSyntaxNode SyntaxNode() => ToClass();
    }
}