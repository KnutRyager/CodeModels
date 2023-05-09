using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public record ParenthesizedPattern(IPattern Pattern)
    : Pattern<ParenthesizedPatternSyntax>
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Pattern;
    }

    public override ParenthesizedPatternSyntax Syntax()
        => SyntaxFactory.ParenthesizedPattern(Pattern.Syntax());
}