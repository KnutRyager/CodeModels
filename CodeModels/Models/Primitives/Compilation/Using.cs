using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record UsingDirective(string Name, bool IsStatic, bool IsGlobal, string? Alias)
    : CodeModel<UsingDirectiveSyntax>()
{
    public static UsingDirective Create(string name,
        bool? isStatic = default,
        bool? isGlobal = default,
        string? alias = default)
        => new(name, isStatic ?? false, isGlobal ?? false, alias);

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