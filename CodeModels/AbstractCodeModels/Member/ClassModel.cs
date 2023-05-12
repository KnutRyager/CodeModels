using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.AbstractCodeModels.Member;

public abstract record ClassModel(string Identifier, NamedValueCollection Properties, List<IMethod> Methods,
     Namespace? Namespace = null, Modifier TopLevelModifier = Modifier.None, Modifier MemberModifier = Modifier.None)
    : AbstractTypeDeclaration<ClassDeclarationSyntax>(Identifier, Properties, Methods, Namespace, TopLevelModifier,
        MemberModifier)
{
    public override ClassDeclarationSyntax Syntax() => ToClass();
}
