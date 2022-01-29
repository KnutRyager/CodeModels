using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

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

    public override void Evaluate(IProgramModelExecutionContext context)
    {
        context.EnterScope();
        Declaration.Evaluate(context);
        while ((bool)Condition.Evaluate(context).LiteralValue)
        {
            Statement.Evaluate(context);
            if (context.HandleReturn() || context.HandleThrow()) return;
            if (context.HandleBreak())  break;
            if (context.HandleContinue())  continue;
            Incrementors.ForEach(x => x.Evaluate(context));
        }
        context.ExitScope();
    }
}

public record SimpleForStatement(string Variable, IExpression Limit, IStatement Statement)
    : ForStatement(
        CodeModelFactory.Declaration(Type("int"), Variable, CodeModelFactory.Literal(0)),
        null,
        BinaryExpression(CodeModelFactory.Identifier(Variable), OperationType.LessThan, Limit),
        UnaryExpression(CodeModelFactory.Identifier(Variable), OperationType.UnaryAddAfter),
        Statement);
