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

        public ValueCollectionWithKey(Value key, IEnumerable<Value> values) : base(values)
        {
            Key = key;
        }

        public ValueCollectionWithKey(Value key, string commaSeparatedValues) : this(key, commaSeparatedValues.Trim().Split(',').Select(x => Value.FromValue(x))) { }
        public ValueCollectionWithKey(Value key, EnumDeclarationSyntax declaration) : this(key, declaration.Members.Select(x => new Value(x))) { }
        public InitializerExpressionSyntax ToKeyValueInitialization() => ComplexElemementExpressionCustom(
            new ExpressionSyntax[] { Key.LiteralExpression, ToArrayInitialization() });
    }
}


