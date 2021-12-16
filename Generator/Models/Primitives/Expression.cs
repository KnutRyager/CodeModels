using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace CodeAnalyzation.Models
{
    public class Expression
    {
        public Value? Value { get; set; }
        public ExpressionSyntax? ExpressionSyntax { get; set; }
        public ExpressionSyntax ExpressionNode => ExpressionSyntax ?? Value?.Expression ?? throw new Exception("Expression has no syntax node or value.");

        public Expression(Value value)
        {
            Value = value;
        }

        public Expression(ExpressionSyntax expression)
        {
            ExpressionSyntax = expression;
        }

        public static Expression FromValue(object literalValue) => new (Value.FromValue(literalValue));
        public static Expression FromQualifiedName(string name) => new(ParseExpression(name));

        public bool IsLiteralExpression => LiteralExpression != null;
        public LiteralExpressionSyntax? LiteralExpression => Value?.LiteralExpression;

        public ArgumentSyntax ToArgument() => ArgumentCustom(
                nameColon: default,
                refKindKeyword: default,
                expression: ExpressionNode);
    }
}