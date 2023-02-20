﻿using System;
using System.Collections.Generic;
using CodeAnalyzation.Models.Execution.Controlflow;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models;

public record ContinueStatement() : AbstractStatement<ContinueStatementSyntax>
{
    public override ContinueStatementSyntax Syntax() => ContinueCustom();

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();

    public override void Evaluate(IProgramModelExecutionContext context)
        => throw new ContinueException();
}
