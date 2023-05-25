using System.Collections.Generic;
using CodeModels.Execution.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record LabeledStatement(string Identifier, IStatement Statement)
    : AbstractStatement<LabeledStatementSyntax>
{
    public static LabeledStatement Create(string identifier, IStatement statement)
        => new(identifier, statement);

    public override LabeledStatementSyntax Syntax() => LabeledStatement(Identifier, Statement.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Statement;
    }

    public override void Evaluate(ICodeModelExecutionContext context) => Statement.Evaluate(context);
}
