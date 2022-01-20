using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

// TODO
public record GotoStatement(IExpression Expression) : AbstractStatement<GotoStatementSyntax>
{
    public override GotoStatementSyntax Syntax() => throw new NotImplementedException();//GotoStatement(SyntaxKind.Block ,Case.Syntax(), Expression.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }
}
