using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IType : ICodeModel<TypeSyntax>
{
    string Name { get; }
    string Identifier { get; }
    bool Required { get; }
    bool IsMulti { get; }
    bool IsStatic { get; }
    TypeSyntax? SourceSyntax { get; }
    new TypeSyntax Syntax();
    Type? ReflectedType { get; }
    TypeSyntax TypeSyntaxNonMultiWrapped();
    TypeSyntax TypeSyntaxNullableWrapped(TypeSyntax type);
    TypeSyntax TypeSyntaxMultiWrapped(TypeSyntax type);
    TypeSyntax TypeSyntaxUnwrapped();
    Type? GetReflectedType();
    IType ToMultiType();
    string GetMostSpecificType();
}
