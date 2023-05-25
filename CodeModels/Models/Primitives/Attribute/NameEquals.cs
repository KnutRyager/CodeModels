using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Attribute;

public record NameEquals(string Identifier) : CodeModel<NameEqualsSyntax>
{
    public static NameEquals Create(string identifier) => new(identifier);

    public override NameEqualsSyntax Syntax() => NameEquals(IdentifierName(Identifier));
    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();
}