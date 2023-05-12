using System;
using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Factory;
using CodeModels.Models;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.AbstractCodeModels.Member;

public abstract record ClassModel(string Identifier, NamedValueCollection Properties, List<IMethod> Methods,
     Namespace? Namespace = null, Modifier TopLevelModifier = Modifier.None, Modifier MemberModifier = Modifier.None)
    : AbstractTypeDeclaration<ClassDeclaration, ClassDeclarationSyntax>(Identifier, Properties, Methods, Namespace, TopLevelModifier,
        MemberModifier)
{
    //IClassDeclaration IAbstractCodeModel<IClassDeclaration>.ToCodeModel(IAbstractCodeModelSettings? settings)
    //    => CodeModelFactory.Class(Identifier, Members(), Namespace, TopLevelModifier);

    protected override ClassDeclaration OnToCodeModel(IAbstractCodeModelSettings? settings = null)
        => CodeModelFactory.Class(Identifier, Members(), Namespace, TopLevelModifier);
}
