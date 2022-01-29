using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models;

public record UnsafeStatement(Block Block) : AbstractStatement<UnsafeStatementSyntax>
{
    public override UnsafeStatementSyntax Syntax() => UnsafeStatement(Block.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Block;
    }

    public override void Evaluate(IProgramModelExecutionContext context) => Block.Evaluate(context);
}
