using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record UsingDirective(string Name, bool IsStatic = false, bool IsGlobal = false, string? Alias = null) : CodeModel<UsingDirectiveSyntax>()
{
    public override UsingDirectiveSyntax Syntax() => UsingDirective(Token(IsGlobal ? SyntaxKind.GlobalKeyword : SyntaxKind.None),
            Token(SyntaxKind.UsingKeyword),
            Token(IsStatic ? SyntaxKind.StaticKeyword : SyntaxKind.None),
            Alias is null ? null : NameEquals(IdentifierName(Alias)),
            IdentifierName(Name),
            Token(SyntaxKind.SemicolonToken));

    public override IEnumerable<ICodeModel> Children()
    {
        throw new NotImplementedException();
    }
}