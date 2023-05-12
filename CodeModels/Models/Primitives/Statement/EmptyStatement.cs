using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System;
using CodeModels.Execution.Context;

namespace CodeModels.Models;

public record EmptyStatement() : AbstractStatement<EmptyStatementSyntax>
{
    public override EmptyStatementSyntax Syntax() => EmptyStatement();

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();

    public override void Evaluate(ICodeModelExecutionContext context)    {   }
}
