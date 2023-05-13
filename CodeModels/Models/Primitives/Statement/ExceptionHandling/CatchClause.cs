using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.ControlFlow;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models;

public record CatchClause(IType Type, string? Identifier, IStatement Statement, CatchFilterClause? Filter) : CodeModel<CatchClauseSyntax>
{
    public static CatchClause Create(IType type, string? identifier, IStatement statement, CatchFilterClause? filter = null)
        => new(type, identifier, statement, filter);

    public override CatchClauseSyntax Syntax() => CatchClauseCustom(
        CatchDeclarationCustom(Type.Syntax(), Identifier), Block(Statement).Syntax(), Filter?.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        yield return Statement;
    }

    public void Evaluate(ThrowException exception, ICodeModelExecutionContext context)
    {
        if (!Match(exception, context)) return;
        context.EnterScope();
        if (Identifier is not null)
        {
            context.DefineVariable(Identifier);
            context.SetValue(Identifier, exception.Expression);
        }
        Statement.Evaluate(context);
        context.ExitScope();
    }

    private bool Match(ThrowException exception, ICodeModelExecutionContext context)
        => Type.IsAssignableFrom(exception.Expression.Get_Type(), context);
}
