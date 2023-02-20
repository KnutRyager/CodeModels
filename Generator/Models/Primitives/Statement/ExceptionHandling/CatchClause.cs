using System.Collections.Generic;
using CodeAnalyzation.Models.Execution.Controlflow;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;

public record CatchClause(IType Type, string? Identifier, IStatement Statement) : CodeModel<CatchClauseSyntax>
{
    public override CatchClauseSyntax Syntax() => CatchClauseCustom(
        CatchDeclarationCustom(Type.Syntax(), Identifier), Block(Statement).Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        yield return Statement;
    }

    public void Evaluate(ThrowException exception, IProgramModelExecutionContext context)
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

    private bool Match(ThrowException exception, IProgramModelExecutionContext context)
        => Type.IsAssignableFrom(exception.Expression.Get_Type(), context);
}
