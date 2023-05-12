using System;
using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.AbstractCodeModels.Member;

public abstract record AbstractTypeDeclaration<T>(string Name, NamedValueCollection Properties, List<IMethod> Methods,
        Namespace? Namespace, Modifier TopLevelModifier,
        Modifier MemberModifier, Type? ReflectedType)
    : AbstractBaseTypeDeclaration<T>(Name, Properties, Methods, Namespace, TopLevelModifier, MemberModifier, ReflectedType),
    ITypeDeclaration<T> where T : TypeDeclarationSyntax
{
    public AbstractTypeDeclaration(string name, NamedValueCollection? properties = null, IEnumerable<IMethod>? methods = null,
        Namespace? @namespace = null, Modifier topLevelModifier = Modifier.Public,
        Modifier memberModifier = Modifier.Public, Type? type = null)
        : this(name, NamedValues(properties), List(methods), @namespace, topLevelModifier, memberModifier, ReflectedType: type)
    {
        foreach (var property in Properties.Properties) property.Owner = this;
    }

    TypeDeclarationSyntax ITypeDeclaration.Syntax() => Syntax();

    T ITypeDeclaration<T>.Syntax() => Syntax();
}
