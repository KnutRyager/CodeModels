using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models.Primitives.Attribute;

public record Attribute(string Name, AttributeArgumentList? Arguments)
    : CodeModel<AttributeSyntax>, IToAttributeConvertible
{
    public static Attribute Create(string name, AttributeArgumentList? arguments = null)
        => new(name, arguments);

    public override AttributeSyntax Syntax() => SyntaxFactory.Attribute(IdentifierName(Name), Arguments?.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        if (Arguments is not null) yield return Arguments;
    }

    public Attribute ToAttribute() => this;
    public AttributeList ToAttributeList() => Attributes(null, new[] { this });
    public AttributeListList ToAttributeListList() => ToAttributeList().ToAttributeListList();

}