using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public interface IType
    {
        string Name { get; }
        string Identifier { get; }
        bool Required { get; }
        bool IsMulti { get; }
        bool IsStatic { get; }
        TypeSyntax? Syntax { get; }
        Type? ReflectedType { get; }
        TypeSyntax TypeSyntax();
        TypeSyntax TypeSyntaxNonMultiWrapped();
        TypeSyntax TypeSyntaxNullableWrapped(TypeSyntax type);
        TypeSyntax TypeSyntaxMultiWrapped(TypeSyntax type);
        TypeSyntax TypeSyntaxUnwrapped();
        Type? GetReflectedType();
        IType ToMultiType();
        string GetMostSpecificType();
    }
}