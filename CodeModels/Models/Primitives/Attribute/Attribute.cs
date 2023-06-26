using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Attribute;

public record Attribute(string Name, AttributeArgumentList? Arguments)
    : CodeModel<AttributeSyntax>
{
    public static Attribute Create(string name, AttributeArgumentList? arguments = null)
        => new(name, arguments);

    public override AttributeSyntax Syntax() => SyntaxFactory.Attribute(IdentifierName(Name), Arguments?.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        if(Arguments is not null) yield return Arguments;
    }
}