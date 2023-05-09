using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Models.CodeModelFactory;

namespace CodeModels.Models;

public abstract record ClassModel(string Identifier, PropertyCollection Properties, List<IMethod> Methods,
     Namespace? Namespace = null, Modifier TopLevelModifier = Modifier.None, Modifier MemberModifier = Modifier.None)
    : TypeDeclaration<ClassDeclarationSyntax>(Identifier, Properties, Methods, Namespace, TopLevelModifier,
        MemberModifier)
{
    public override ClassDeclarationSyntax Syntax() => ToClass();
}
