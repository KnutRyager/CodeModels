using System;
using Common.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public record TypeFromReflection(Type Type, bool Required = true, bool IsMulti = false, TypeSyntax? Syntax = null)
        : AbstractType(ReflectionSerialization.GetToShortHandName(Type.Name), Required, IsMulti, Syntax, Type)
    {
    }
}