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
    new TypeSyntax Syntax();
    Type? ReflectedType { get; }
    TypeSyntax TypeSyntaxNonMultiWrapped();
    TypeSyntax TypeSyntaxNullableWrapped(TypeSyntax type);
    TypeSyntax TypeSyntaxMultiWrapped(TypeSyntax type);
    TypeSyntax TypeSyntaxUnwrapped();
    TypeParameterSyntax ToTypeParameter();
    Type? GetReflectedType();
    IType ToMultiType();
    string GetMostSpecificType();
    IType GetGenericType(int index);
}
