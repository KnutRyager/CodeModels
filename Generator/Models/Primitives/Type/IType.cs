using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public interface IType
    {
        string Identifier { get; }
        bool Required { get; }
        bool IsMulti { get; }
        TypeSyntax? Syntax { get; }
        Type? Type { get; }
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