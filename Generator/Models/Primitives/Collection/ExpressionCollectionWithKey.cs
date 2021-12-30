using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models
{
    public record ExpressionCollectionWithKey(Expression Key, List<Expression> Values, bool MultiValues = false, IType? ValueType = null) : ExpressionCollection(Values)
    {
        private readonly IType? _valueType;

        public ExpressionCollectionWithKey(Expression key, IEnumerable<Expression> values, bool multiValues = false, IType? valueType = null)
            : this(key, values.ToList(), multiValues || (values.Count() is not 1))
        {
            _valueType = valueType;
        }

        public ExpressionCollectionWithKey(Expression key, Expression value, bool multiValues = false, AbstractType? valueType = null) 
            : this(key, new[] { value }, multiValues, valueType) { }

        public ExpressionCollectionWithKey(Expression key, string commaSeparatedValues) : this(key, commaSeparatedValues.Trim().Split(',').Select(x => new LiteralExpression(x))) { }
        public ExpressionCollectionWithKey(Expression key, EnumDeclarationSyntax declaration) : this(key, declaration.Members.Select(x => new LiteralExpression(x))) { }
        public InitializerExpressionSyntax ToKeyValueInitialization() => ComplexElemementExpressionCustom(
            new ExpressionSyntax[] { Key.Syntax, MultiValues ? ToArrayInitialization() : Values.First().Syntax });

        public override IType BaseTType() => _valueType ?? (MultiValues ? base.BaseTType().ToMultiType() : base.BaseTType());
    }
}


