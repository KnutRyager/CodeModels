using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Attribute;

public record NameColon(string Name) : CodeModel<NameColonSyntax>
{
    public static NameColon Create(string name) => new(name);

    public override NameColonSyntax Syntax() => NameColon(IdentifierName(Name));
    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();
}
