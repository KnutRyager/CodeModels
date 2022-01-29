using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IExpression : ICodeModel
{
    IType Type { get; }
    bool IsLiteralExpression { get; }
    LiteralExpressionSyntax? LiteralSyntax { get; }
    ArgumentSyntax ToArgument();
    object? LiteralValue { get; }
    IExpression Evaluate(IProgramModelExecutionContext context);
    object? EvaluatePlain(IProgramModelExecutionContext context);
    EnumMemberDeclarationSyntax ToEnumValue(int? value = null);
    ExpressionStatement AsStatement();

    new ExpressionSyntax Syntax();
}

public interface IExpression<T> : IExpression, ICodeModel<T> where T : ExpressionSyntax
{
    new T Syntax();
}
