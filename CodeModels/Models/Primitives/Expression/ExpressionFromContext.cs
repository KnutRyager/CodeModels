using System.Collections.Generic;
using CodeModels.Execution;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record ExpressionFromTypeContext(IProgramContext Context, IType Type) : Expression<ExpressionSyntax>(Type)
//public record ExpressionFromTypeContext(IProgramContext Context, IType Type) : Expression<ExpressionSyntax>(Type, Context: Context)
{
    public IExpression Resolve() => Context!.GetSingleton(Type);
    public override ExpressionSyntax Syntax() => Resolve().Syntax();
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }
}
