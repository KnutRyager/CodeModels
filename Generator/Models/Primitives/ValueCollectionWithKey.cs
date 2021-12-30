using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public class ValueCollectionWithKey : ValueCollection
    {
        public Value Key { get; set; }
        public bool MultiValues { get; set; }
        private readonly TType? _valueType;

        public ValueCollectionWithKey(Value key, IEnumerable<Value> values, bool multiValues = false, TType? valueType = null) : base(values)
        {
            Key = key;
            MultiValues = (values.Count() is not 1) || multiValues;
            _valueType = valueType;
        }
        public ValueCollectionWithKey(Value key, Value value, bool multiValues = false, TType? valueType = null) : this(key, new[] { value }, multiValues, valueType) { }

        public ValueCollectionWithKey(Value key, string commaSeparatedValues) : this(key, commaSeparatedValues.Trim().Split(',').Select(x => Value.FromValue(x))) { }
        public ValueCollectionWithKey(Value key, EnumDeclarationSyntax declaration) : this(key, declaration.Members.Select(x => new Value(x))) { }
        public InitializerExpressionSyntax ToKeyValueInitialization() => ComplexElemementExpressionCustom(
            new ExpressionSyntax[] { Key.Expression, MultiValues ? ToArrayInitialization() : Values.First().Expression });

        public override TType BaseTType() => _valueType ?? (MultiValues ? TType.MultiType(base.BaseTType()) : base.BaseTType());
    }
}


