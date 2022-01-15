using System;
using Common.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public record TypeFromReflection(Type ReflectedType, bool Required = true, bool IsMulti = false, TypeSyntax? SourceSyntax = null)
    : AbstractType(ReflectionSerialization.GetToShortHandName(ReflectedType.Name), Required, IsMulti, SourceSyntax, ReflectedType)
{
}
