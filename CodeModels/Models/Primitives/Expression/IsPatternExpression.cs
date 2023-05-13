using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace CodeModels.Models;

public record IsPatternExpression(IExpression Lhs, IPattern Pattern,IType Type)
    : Expression<IsPatternExpressionSyntax>(Type)
{

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Lhs;
        yield return Pattern;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }

    public override object? EvaluatePlain(ICodeModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }


    public override IsPatternExpressionSyntax Syntax()
        => SyntaxFactory.IsPatternExpression(Lhs.Syntax().WithTrailingTrivia(SyntaxFactory.Space), Pattern.Syntax());
}
