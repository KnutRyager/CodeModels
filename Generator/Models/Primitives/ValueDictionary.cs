using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public class ValueDictionary
    {
        public List<ValueCollectionWithKey> KeyVaulePairs { get; set; }
        public ValueCollection Keys { get; set; }
        public ValueCollection Values { get; set; }

        public ValueDictionary(IEnumerable<ValueCollectionWithKey> values)
        {
            KeyVaulePairs = values.ToList();
            Keys = new ValueCollection(values.Select(x => x.Key));
            Values = new ValueCollection(values.SelectMany(x => x.Values));
        }

        public ObjectCreationExpressionSyntax ToDictionary() => DictionaryCreationExpressionCustom(
            keyType: BaseKeyType(),
            valueType: BaseValueType(),
            argumentList: default,
            initializer: CollectionInitializerExpressionCustom(KeyVaulePairs.Select(x => x.ToKeyValueInitialization()).ToList()));

        public TypeSyntax BaseKeyType() => Keys.BaseType();
        public TypeSyntax BaseValueType() => Values.BaseType();

    }
}


