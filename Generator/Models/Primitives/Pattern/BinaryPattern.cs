using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public abstract record BinaryPattern(SyntaxKind Kind, IPattern Left, IPattern Right) 
    : Pattern<BinaryPatternSyntax>
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Left;
        yield return Right;
    }

    public override BinaryPatternSyntax Syntax()
        => SyntaxFactory.BinaryPattern(Kind, Left.Syntax(), Right.Syntax());
}