using System;
using System.Collections.Generic;
using CodeModels.Execution;
using CodeModels.Execution.ControlFlow;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models;

public record ContinueStatement() : AbstractStatement<ContinueStatementSyntax>
{
    public override ContinueStatementSyntax Syntax() => ContinueCustom();

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();

    public override void Evaluate(IProgramModelExecutionContext context)
        => throw new ContinueException();
}
