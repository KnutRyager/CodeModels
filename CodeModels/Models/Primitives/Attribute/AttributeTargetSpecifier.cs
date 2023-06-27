using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Attribute;

public record AttributeTargetSpecifier(string Identifier) 
    : CodeModel<AttributeTargetSpecifierSyntax>
{
    public static AttributeTargetSpecifier Create(string identifier) => new(identifier);

    public override AttributeTargetSpecifierSyntax Syntax() => SyntaxFactory.AttributeTargetSpecifier(Identifier(Identifier));
    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();
}
