using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace CodeAnalyzation.Models
{
    public class ExpressionFromSyntax : Expression
    {
        public ExpressionSyntax ExpressionSyntax { get; set; }

        public ExpressionFromSyntax(ExpressionSyntax syntax) : base(TType.NullType)
        {
            ExpressionSyntax = syntax;
        }

        public ExpressionFromSyntax(string qualifiedName) : this(ParseExpression(qualifiedName)) { }

        public override ExpressionSyntax Syntax => ExpressionSyntax ?? base.Syntax;
    }
}