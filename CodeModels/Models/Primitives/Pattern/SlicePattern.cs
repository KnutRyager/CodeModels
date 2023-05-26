using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record SlicePattern(IPattern? Pattern)
    : Pattern<SlicePatternSyntax>
{
    public static SlicePattern Create(IPattern? pattern = default) => new(pattern);

    public override IEnumerable<ICodeModel> Children()
    {
        if (Pattern is not null) yield return Pattern;
    }

    public override SlicePatternSyntax Syntax()
        => SyntaxFactory.SlicePattern(Pattern?.Syntax());
}