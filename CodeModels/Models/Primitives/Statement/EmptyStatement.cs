using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeModels.Generation.SyntaxFactoryCustom;
using System;
using CodeModels.Execution;

namespace CodeModels.Models;

public record EmptyStatement() : AbstractStatement<EmptyStatementSyntax>
{
    public override EmptyStatementSyntax Syntax() => EmptyStatement();

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();

    public override void Evaluate(IProgramModelExecutionContext context)    {   }
}
