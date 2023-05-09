﻿using System;
using CodeModels.Execution.Context;
using Common.DataStructures;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IType : ICodeModel<TypeSyntax>, IExpression, IMember
{
    string TypeName { get; }
    bool Required { get; }
    bool IsMulti { get; }
    new TypeSyntax Syntax();
    Type? ReflectedType { get; }
    IType PlainType();
    TypeSyntax TypeSyntaxNonMultiWrapped();
    TypeSyntax TypeSyntaxNullableWrapped(TypeSyntax type);
    TypeSyntax TypeSyntaxMultiWrapped(TypeSyntax type);
    TypeSyntax TypeSyntaxUnwrapped();
    TypeParameterSyntax ToTypeParameter();
    Type? GetReflectedType();
    IType ToMultiType();
    string GetMostSpecificType();
    IType GetGenericType(int index);
    EqualityList<IType> GenericTypes { get; }
    bool Equals(IType other, ICodeModelExecutionContext context);
    bool IsAssignableFrom(IType other, ICodeModelExecutionContext context);
}
