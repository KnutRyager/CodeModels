using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public record UnaryPattern(IPattern Pattern)
    : Pattern<UnaryPatternSyntax>
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Pattern;
    }

    public override UnaryPatternSyntax Syntax()
        => SyntaxFactory.UnaryPattern(Pattern.Syntax());
}
