using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Factory;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record GlobalStatement(IStatement Statement)
    : MemberModel<GlobalStatementSyntax>(TypeShorthands.VoidType,
        CodeModelFactory.AttributesList(),  // TODO
        Modifier.None)
{
    public override GlobalStatementSyntax Syntax() => SyntaxFactory.GlobalStatement(Statement.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Statement;
    }

    public void Evaluate(ICodeModelExecutionContext context)
    {
        Statement.Evaluate(context);
    }

    public override GlobalStatementSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
        => Syntax();

    public override CodeModel<GlobalStatementSyntax> Render(Namespace @namespace)
        => this;
}
