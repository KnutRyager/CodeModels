using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace CodeAnalyzation.Models
{
    public record PropertyExpression(Property Property, IExpression? Instance = null) : Expression(Property.Type)
    {
        public override ExpressionSyntax Syntax => Property?.AccessSyntax(Instance) ?? base.Syntax;
    }
}