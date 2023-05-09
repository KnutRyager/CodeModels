using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IExpressionOrPattern : ICodeModel, IIdentifiable
{
    IType Get_Type();
    IExpression Evaluate(IProgramModelExecutionContext context);
    object? EvaluatePlain(IProgramModelExecutionContext context);
    new ExpressionOrPatternSyntax Syntax();
}

public interface IExpressionOrPattern<T> : IExpressionOrPattern, ICodeModel<T> where T : ExpressionOrPatternSyntax
{
    new T Syntax();
}
