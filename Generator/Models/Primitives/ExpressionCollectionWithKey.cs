using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public class ExpressionCollectionWithKey : ExpressionCollection
    {
        public Expression Key { get; set; }
        public bool MultiValues { get; set; }
        private readonly TType? _valueType;

        public ExpressionCollectionWithKey(Expression key, IEnumerable<Expression> values, bool multiValues = false, TType? valueType = null) : base(values)
        {
            Key = key;
            MultiValues = (values.Count() is not 1) || multiValues;
            _valueType = valueType;
        }
        public ExpressionCollectionWithKey(Expression key, Expression value, bool multiValues = false, TType? valueType = null) : this(key, new[] { value }, multiValues, valueType) { }

        public ExpressionCollectionWithKey(Expression key, string commaSeparatedValues) : this(key, commaSeparatedValues.Trim().Split(',').Select(x => new LiteralExpression(x))) { }
        public ExpressionCollectionWithKey(Expression key, EnumDeclarationSyntax declaration) : this(key, declaration.Members.Select(x => new LiteralExpression(x))) { }
        public InitializerExpressionSyntax ToKeyValueInitialization() => ComplexElemementExpressionCustom(
            new ExpressionSyntax[] { Key.Syntax, MultiValues ? ToArrayInitialization() : Values.First().Syntax });

        public override TType BaseTType() => _valueType ?? (MultiValues ? TType.MultiType(base.BaseTType()) : base.BaseTType());
    }
}


