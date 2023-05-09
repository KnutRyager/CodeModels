using System;
using System.Collections.Generic;
using CodeModels.Models.Execution.ControlFlow;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models;

public record BreakStatement() : AbstractStatement<BreakStatementSyntax>
{
    public override BreakStatementSyntax Syntax() => BreakCustom();

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();

    public override void Evaluate(IProgramModelExecutionContext context) => throw new BreakException();
}
