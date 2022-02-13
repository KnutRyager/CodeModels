using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;

public abstract record ClassModel(string Identifier, PropertyCollection Properties, List<IMethod> Methods,
     Namespace? Namespace = null, Modifier TopLevelModifier = Modifier.None, Modifier MemberModifier = Modifier.None)
    : MethodHolder<ClassDeclarationSyntax>(Identifier, Properties, Methods, Namespace, TopLevelModifier,
        MemberModifier)
{
    public override ClassDeclarationSyntax Syntax() => ToClass();
}
