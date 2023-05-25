using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Expression.Reference;

public record BaseExpression(IType Type) : Expression<BaseExpressionSyntax>(Type)
{
    public static BaseExpression Create(IType type) => new(type);

    public override BaseExpressionSyntax Syntax() => BaseExpression();

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override object? EvaluatePlain(ICodeModelExecutionContext context) => context.Base().EvaluatePlain(context);
    public override IExpression Evaluate(ICodeModelExecutionContext context) => context.Base().Evaluate(context);
}