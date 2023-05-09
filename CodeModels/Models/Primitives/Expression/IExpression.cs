using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IExpression : ICodeModel, IIdentifiable, IExpressionOrPattern
{
    bool IsLiteralExpression { get; }
    LiteralExpressionSyntax? LiteralSyntax();
    ArgumentSyntax ToArgument();
    object? LiteralValue();
    EnumMemberDeclarationSyntax ToEnumValue(int? value = null);
    //System.Type GetReflectedType();
    ExpressionStatement AsStatement();
    new ExpressionSyntax Syntax();
}

public interface IExpression<T> : IExpression, IExpressionOrPattern<T>, ICodeModel<T> where T : ExpressionSyntax
{
    new T Syntax();
}
