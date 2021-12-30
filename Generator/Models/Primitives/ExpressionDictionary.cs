using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public class ExpressionDictionary
    {
        public List<ExpressionCollectionWithKey> KeyVaulePairs { get; set; }
        public ExpressionCollection Keys { get; set; }
        public ExpressionCollection Values { get; set; }
        public string? Name { get; set; }
        private readonly TType? _valueType;

        public ExpressionDictionary(IEnumerable<ExpressionCollectionWithKey> values, string? name = null, TType? valueType = null)
        {
            KeyVaulePairs = values.ToList();
            Keys = new ExpressionCollection(values.Select(x => x.Key));
            Values = new ExpressionCollection(values.SelectMany(x => x.Values));
            Name = name;
            _valueType = valueType;
        }

        public ObjectCreationExpressionSyntax ToDictionary() => DictionaryCreationExpressionCustom(
            keyType: BaseKeyType(),
            valueType: BaseValueType(),
            argumentList: default,
            initializer: CollectionInitializerExpressionCustom(KeyVaulePairs.Select(x => x.ToKeyValueInitialization()).ToList()));

        public TypeSyntax ToDictionaryType() => GenericName(Identifier("IDictionary"), TypeArgumentListCustom(BaseKeyType(), BaseValueType()));

        public Property ToProperty(string? name = null) => new(ToDictionaryType(), name ?? Name ?? throw new ArgumentException($"No name for property"), ToDictionary(), modifier: Modifier.Readonly | Modifier.Public);

        public TypeSyntax BaseKeyType() => Keys.BaseType();
        public TypeSyntax BaseValueType() => (_valueType ?? FindCommonType(KeyVaulePairs.Select(x => x.BaseTType()))).TypeSyntax();

        public static TType FindCommonType(IEnumerable<TType> types)
        {
            var isMulti = types.Any(x => x.IsMulti);
            var disinctTypes = types.Select(x => x.Identifier).Distinct();
            var specificType = disinctTypes.Count() == 1 ? types.First().GetMostSpecificType()
                : disinctTypes.Count() == 0 ? "int"
                : types.Any(x => x.GetMostSpecificType() is "object") ? "object"
                : types.Any(x => x.GetMostSpecificType() is "string") ? "string"
                : "int";
            var optional = types.Any(x => !x.Required);
            return new TType(specificType, !optional, isMulti);
        }

    }
}


