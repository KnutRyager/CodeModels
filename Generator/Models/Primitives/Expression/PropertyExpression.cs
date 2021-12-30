using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace CodeAnalyzation.Models
{
    public class PropertyExpression : Expression
    {
        public Property? Property { get; set; }
        public IExpression? Instance { get; set; }

        public PropertyExpression(Property property, IExpression? instance = null) : base(property.Type)
        {
            Property = property;
            Instance = instance;
        }

        public override ExpressionSyntax Syntax => Property?.AccessSyntax(Instance) ?? base.Syntax;
    }
}