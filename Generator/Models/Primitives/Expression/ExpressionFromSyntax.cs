using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace CodeAnalyzation.Models
{
    public record ExpressionFromSyntax(ExpressionSyntax ExpressionSyntax) : Expression(TypeShorthands.NullType)
    {
        public ExpressionFromSyntax(string qualifiedName) : this(ParseExpression(qualifiedName)) { }
        public override ExpressionSyntax Syntax => ExpressionSyntax ?? base.Syntax;
    }
}