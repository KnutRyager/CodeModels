using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record LabeledStatement(string Identifier, IStatement Statement) : AbstractStatement<LabeledStatementSyntax>
{
    public override LabeledStatementSyntax Syntax() => LabeledStatement(Identifier, Statement.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Statement;
    }
}
