using System;
using System.Collections.Generic;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Attribute;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public abstract record TypeDeclaration<T>(string Name,
    AttributeListList Attributes,
    List<IType> GenericParameters,
    List<TypeParameterConstraintClause> ConstraintClauses,
    List<IBaseType> BaseTypeList,
    List<IMember> Members,
    Namespace? Namespace,
    Modifier TopLevelModifier,
    Modifier MemberModifier,
    Type? ReflectedType)
    : BaseTypeDeclaration<T>(Name, Attributes, GenericParameters, ConstraintClauses, BaseTypeList, Members, Namespace, TopLevelModifier, MemberModifier, ReflectedType),
    ITypeDeclaration<T>,
    IScopeHolder where T : TypeDeclarationSyntax

{
    BaseTypeDeclarationSyntax IBaseTypeDeclaration.Syntax() => Syntax();
    TypeDeclarationSyntax ITypeDeclaration.Syntax() => Syntax();
}
