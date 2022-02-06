using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IExpression : ICodeModel, IIdentifiable
{
    IType Get_Type();
    bool IsLiteralExpression { get; }
    LiteralExpressionSyntax? LiteralSyntax { get; }
    ArgumentSyntax ToArgument();
    object? LiteralValue { get; }
    IExpression Evaluate(IProgramModelExecutionContext context);
    object? EvaluatePlain(IProgramModelExecutionContext context);
    EnumMemberDeclarationSyntax ToEnumValue(int? value = null);
    //System.Type GetReflectedType();
    ExpressionStatement AsStatement();

    new ExpressionSyntax Syntax();
}

public interface IExpression<T> : IExpression, ICodeModel<T> where T : ExpressionSyntax
{
    new T Syntax();
}
