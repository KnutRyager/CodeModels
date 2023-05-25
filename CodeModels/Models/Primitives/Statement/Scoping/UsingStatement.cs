using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record UsingStatement(IStatement Statement, VariableDeclarations? Declarations = null, IExpression? Expression = null) : AbstractStatement<UsingStatementSyntax>
{
    public override UsingStatementSyntax Syntax() => UsingStatement(
        Declarations?.Syntax(),
        Expression?.Syntax(),
        Statement.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Statement;
    }

    public override void Evaluate(ICodeModelExecutionContext context)
    {
        context.EnterScope();
        if (Declarations is not null)
        {
            try
            {
                Declarations.Evaluate(context);
                Statement.Evaluate(context);
            }
            finally
            {
                foreach (var declaration in Declarations.Value)
                {
                    if (declaration is VariableDeclarator declarator)
                    {
                        var disposableCandidate = context.GetValue(declarator.Name)?.LiteralValue();
                        if (disposableCandidate is System.IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                }
            }

        }
        else if (Expression is not null)
        {
            try
            {
                Expression.Evaluate(context);
                Statement.Evaluate(context);
            }
            finally
            {
                if (Expression is AssignmentExpression { Kind: AssignmentType.Simple, Left: IdentifierExpression identifier })
                {
                    var disposableCandidate = context.GetValue(identifier.Name)?.LiteralValue();
                    if (disposableCandidate is System.IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }
        context.ExitScope();
    }
}
