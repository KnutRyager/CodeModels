using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record ExternAliasDirective(string Name)
    : CodeModel<ExternAliasDirectiveSyntax>()
{
    public override ExternAliasDirectiveSyntax Syntax()
        => ExternAliasDirective(Name);

    public override IEnumerable<ICodeModel> Children()
        => Enumerable.Empty<ICodeModel>();
}