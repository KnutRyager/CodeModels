using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace CodeAnalyzation.Models
{
    public record ExpressionFromSyntax(ExpressionSyntax SourceSyntax) : Expression<ExpressionSyntax>(TypeShorthands.NullType)
    {
        public ExpressionFromSyntax(string qualifiedName) : this(ParseExpression(qualifiedName)) { }

        public override ExpressionSyntax Syntax() => SourceSyntax ?? ((IExpression)this).Syntax();
    }
}