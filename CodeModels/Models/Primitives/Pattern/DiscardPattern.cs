using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record DiscardPattern()
    : Pattern<DiscardPatternSyntax>
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield break;
    }

    public override DiscardPatternSyntax Syntax()
        => SyntaxFactory.DiscardPattern();
}