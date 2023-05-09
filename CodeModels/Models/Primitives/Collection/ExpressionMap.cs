using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.Factory;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record ExpressionMap(List<ExpressionsMap> KeyVaulePairs, ExpressionCollection Keys, ExpressionCollection Values, string? Name)
{
    private readonly AbstractType? _valueType;

    public ExpressionMap(IEnumerable<ExpressionsMap> values, string? name = null, AbstractType? valueType = null)
        : this(values.ToList(), new ExpressionCollection(values.Select(x => x.Key)), new ExpressionCollection(values.SelectMany(x => x.Values)), name)
    {
        _valueType = valueType;
    }

    public ObjectCreationExpressionSyntax ToDictionary()
    {
        var fuk = DictionaryCreationExpressionCustom(
keyType: BaseKeyTypeSyntax(),
valueType: BaseValueTypeSyntax(),
argumentList: default,
initializer: CollectionInitializerExpressionCustom(KeyVaulePairs.Select(x => x.ToKeyValueInitialization()).ToList()));
        return DictionaryCreationExpressionCustom(
keyType: BaseKeyTypeSyntax(),
valueType: BaseValueTypeSyntax(),
argumentList: default,
initializer: CollectionInitializerExpressionCustom(KeyVaulePairs.Select(x => x.ToKeyValueInitialization()).ToList()));
    }

    public IType ToDictionaryType() => CodeModelFactory.QuickType("Dictionary", new[] { BaseKeyType(), BaseValueType() });
    public IType ToDictionaryInterfaceType() => CodeModelFactory.QuickType("IDictionary", new[] { BaseKeyType(), BaseValueType() });

    public Property ToProperty(string? name = null) => new(ToDictionaryType().Syntax(), name ?? Name ?? throw new ArgumentException($"No name for property"), ToDictionary(), modifier: Modifier.Readonly | Modifier.Public, interfaceType: ToDictionaryInterfaceType().Syntax());
    //public Property ToProperty(string? name = null) => new(ToDictionaryType(), name ?? Name ?? throw new ArgumentException($"No name for property"), ToDictionary(), modifier: Modifier.Readonly | Modifier.Public, interfaceType: ToDictionaryInterfaceType());

    public IType BaseKeyType() => Keys.BaseType();
    public TypeSyntax BaseKeyTypeSyntax() => BaseKeyType().Syntax();
    public IType BaseValueType() => (_valueType ?? TypeUtil.FindCommonType(KeyVaulePairs.Select(x => x.BaseType())));
    public TypeSyntax BaseValueTypeSyntax() => BaseValueType().Syntax();

}


