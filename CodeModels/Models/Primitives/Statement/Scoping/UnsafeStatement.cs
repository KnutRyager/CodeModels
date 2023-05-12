using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using CodeModels.Execution.Context;

namespace CodeModels.Models;

public record UnsafeStatement(Block Block) : AbstractStatement<UnsafeStatementSyntax>
{
    public override UnsafeStatementSyntax Syntax() => UnsafeStatement(Block.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Block;
    }

    public override void Evaluate(ICodeModelExecutionContext context) => Block.Evaluate(context);
}
