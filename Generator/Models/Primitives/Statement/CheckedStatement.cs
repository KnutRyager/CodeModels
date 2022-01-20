using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record CheckedStatement(Block Block) : AbstractStatement<CheckedStatementSyntax>
{
    public override CheckedStatementSyntax Syntax() => CheckedStatement(SyntaxKind.Block, Block.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Block;
    }
}
