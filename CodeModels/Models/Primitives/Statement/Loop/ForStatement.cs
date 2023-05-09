using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using CodeModels.Factory;
using CodeModels.Execution.ControlFlow;
using CodeModels.Execution.Context;

namespace CodeModels.Models;

public record ForStatement(VariableDeclarations Declaration, List<IExpression> Initializers, IExpression Condition, List<IExpression> Incrementors, IStatement Statement) : AbstractStatement<ForStatementSyntax>
{
    public ForStatement(VariableDeclaration? declaration, IExpression? initializers, IExpression condition, IExpression incrementor, IStatement statement)
        : this(new(declaration), initializers is null ? List<IExpression>() : List(initializers), condition, List(incrementor), statement) { }

    public override ForStatementSyntax Syntax() => ForStatementCustom(Declaration.Syntax(),
        Initializers.Select(x => x.Syntax()),
        Condition.Syntax(),
        Incrementors.Select(x => x.Syntax()),
        Statement.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Declaration;
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
            Declaration.Evaluate(context);
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

public record SimpleForStatement(string Variable, IExpression Limit, IStatement Statement)
    : ForStatement(
        CodeModelFactory.Declaration(Type("int"), Variable, CodeModelFactory.Literal(0)),
        null,
        BinaryExpression(CodeModelFactory.Identifier(Variable), OperationType.LessThan, Limit),
        UnaryExpression(CodeModelFactory.Identifier(Variable), OperationType.UnaryAddAfter),
        Statement);
