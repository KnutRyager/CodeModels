using System;
using System.Collections.Generic;
using CodeModels.Models.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public abstract record BaseType<T>(string Name,
    List<IMember> Members,
    Namespace? Namespace,
    Modifier TopLevelModifier,
    Modifier MemberModifier,
    Type? ReflectedType)
    : BaseBaseType<T>(Name, Members, Namespace, TopLevelModifier, MemberModifier, ReflectedType),
    ITypeDeclaration<T>,
    IScopeHolder
    where T : TypeDeclarationSyntax

{
    BaseTypeDeclarationSyntax IBaseTypeDeclaration.Syntax() => Syntax();
    TypeDeclarationSyntax ITypeDeclaration.Syntax() => Syntax();
}
