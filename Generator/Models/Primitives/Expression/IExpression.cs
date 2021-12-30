using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace CodeAnalyzation.Models
{
    public interface IExpression
    {
        bool IsLiteralExpression { get; }
        LiteralExpressionSyntax? LiteralSyntax { get; }
        ArgumentSyntax ToArgument();
        ExpressionSyntax Syntax { get; }
    }
}