using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record ExpressionFromTypeContext(IProgramContext Context, IType Type)
    : Expression<ExpressionSyntax>(Type)
{
    public IExpression Resolve() => Context!.GetSingleton(Type);
    public override ExpressionSyntax Syntax() => Resolve().Syntax();
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }
}
