﻿using System;
using System.Collections.Generic;
using CodeModels.Models.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public abstract record TypeDeclaration<T>(string Name,
    List<IType> GenericParameters,
    List<TypeParameterConstraintClause> ConstraintClauses,
    List<IBaseType> BaseTypeList,
    List<IMember> Members,
    Namespace? Namespace,
    Modifier TopLevelModifier,
    Modifier MemberModifier,
    Type? ReflectedType)
    : BaseTypeDeclaration<T>(Name, GenericParameters, ConstraintClauses, BaseTypeList, Members, Namespace, TopLevelModifier, MemberModifier, ReflectedType),
    ITypeDeclaration<T>,
    IScopeHolder where T : TypeDeclarationSyntax

{
    BaseTypeDeclarationSyntax IBaseTypeDeclaration.Syntax() => Syntax();
    TypeDeclarationSyntax ITypeDeclaration.Syntax() => Syntax();
}
