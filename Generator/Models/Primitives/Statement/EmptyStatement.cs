using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using System;

namespace CodeAnalyzation.Models;

public record EmptyStatement() : AbstractStatement<EmptyStatementSyntax>
{
    public override EmptyStatementSyntax Syntax() => EmptyStatement();

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();

    public override void Evaluate(IProgramModelExecutionContext context)    {   }
}
