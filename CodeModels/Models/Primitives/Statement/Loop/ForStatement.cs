using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution.Context;
using CodeModels.Execution.ControlFlow;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record ForStatement(VariableDeclarations? Declaration, List<IExpression> Initializers, IExpression Condition, List<IExpression> Incrementors, IStatement Statement) : AbstractStatement<ForStatementSyntax>
{
    public static ForStatement Create(VariableDeclarations? declarations, IEnumerable<IExpression>? initializers, IExpression condition, IEnumerable<IExpression> incrementors, IStatement statement)
        => new(declarations, List(initializers), condition, List(incrementors), statement);

    public static ForStatement Create(VariableDeclaration? declaration, IExpression? initializer, IExpression condition, IExpression incrementor, IStatement statement)
        => Create(VariableDeclarations(declaration), initializer is null ? null : List(initializer), condition, incrementor is null ? new List<IExpression>() : List(incrementor), statement);

    public override ForStatementSyntax Syntax()
        => SyntaxFactory.ForStatement(Declaration?.Syntax(),
        SeparatedList(Initializers.Select(x => x.Syntax())),
        Condition.Syntax(),
        SeparatedList(Incrementors.Select(x => x.Syntax())),
        Statement.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        if (Declaration is not null) yield return Declaration;
        yield return Condition;
        foreach (var incrementor in Incrementors) yield return incrementor;
        foreach (var initializer in Initializers) yield return initializer;
        yield return Statement;
    }

    public override void Evaluate(ICodeModelExecutionContext context)
    {
        context.EnterScope();
        try
        {
            context.IncreaseDisableSetPreviousValueLock();
            Declaration?.Evaluate(context);
        }
        finally
        {
            context.DecreaseDisableSetPreviousValueLock();
        }
        while (((bool?)Condition.Evaluate(context).LiteralValue()) ?? false)
        {
            try
            {
                Statement.Evaluate(context);
            }
            catch (BreakException)
            {
                break;
            }
            catch (ContinueException)
            {
                Incrementors.ForEach(x => x.Evaluate(context));
                continue;
            }
            try
            {
                context.IncreaseDisableSetPreviousValueLock();
                Incrementors.ForEach(x => x.Evaluate(context));
            }
            finally
            {
                context.DecreaseDisableSetPreviousValueLock();
            }
        }
        context.ExitScope();
    }

    public override string ToString() => $"for({Declaration}{Initializers},{Incrementors}{Condition}){{{Statement}}}";
}