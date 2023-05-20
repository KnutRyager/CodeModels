using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution.Context;
using CodeModels.Factory;
using CodeModels.Generation;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

// TODO: Determine arguments vs initializer
public record InitializerExpression(IType Type, SyntaxKind Kind, List<IExpression> Expressions)
    : Expression<InitializerExpressionSyntax>(Type)
{
    public override InitializerExpressionSyntax Syntax()
        => InitializerExpression(Kind, SeparatedList(Expressions.Select(x => x.Syntax())));

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        foreach (var expression in Expressions) yield return expression;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        var evaluatedExpressions = Expressions.Select(x => x.EvaluatePlain(context));
        return Kind switch
        {
            SyntaxKind.ArrayInitializerExpression => CodeModelFactory.Literal(evaluatedExpressions.ToArray()),
            _ => CodeModelFactory.Literal(evaluatedExpressions.ToArray())
            //_ => throw new NotImplementedException()
        };
    }
}
