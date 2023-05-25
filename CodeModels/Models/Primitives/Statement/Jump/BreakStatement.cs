using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.ControlFlow;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models;

public record BreakStatement() : AbstractStatement<BreakStatementSyntax>
{
    public static BreakStatement Create() => new();

    public override BreakStatementSyntax Syntax() => BreakCustom();

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();

    public override void Evaluate(ICodeModelExecutionContext context) => throw new BreakException();
}
