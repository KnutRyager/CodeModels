﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.AbstractCodeModels.Member;
using CodeModels.Factory;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Instantiation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.AbstractCodeModels.Collection;

public record ExpressionMap(List<ExpressionsMap> KeyVaulePairs, ExpressionCollection Keys, ExpressionCollection Values, string? Name)
{
    private readonly AbstractType? _valueType;

    public static ExpressionMap Create(IEnumerable<ExpressionsMap> keyVaulePairs, ExpressionCollection keys, ExpressionCollection values, string? name = null)
        => new(List(keyVaulePairs), keys, values, name);
    public static ExpressionMap Create(IEnumerable<ExpressionsMap> values, string? name = null, AbstractType? valueType = null)
        => new(values, name, valueType);

    private ExpressionMap(IEnumerable<ExpressionsMap> values, string? name = null, AbstractType? valueType = null)
        : this(values.ToList(), AbstractCodeModelFactory.Expressions(values.Select(x => x.Key)), AbstractCodeModelFactory.Expressions(values.SelectMany(x => x.Values)), name)
    {
        _valueType = valueType;
    }

    public ObjectCreationExpressionSyntax ToDictionary() => DictionaryCreationExpressionCustom(
        keyType: BaseKeyTypeSyntax(),
        valueType: BaseValueTypeSyntax(),
        argumentList: default,
        initializer: CollectionInitializerExpressionCustom(KeyVaulePairs.Select(x => x.ToKeyValueInitializationSyntax()).ToList()));

    public ObjectCreationExpression ToDictionary2()
        => CodeModelFactory.ObjectCreation(ToDictionaryType(),
            arguments: new List<IExpression>(),
            initializer: CodeModelFactory.CollectionInitializer(KeyVaulePairs.Select(x => x.ToKeyValueInitialization()).ToList(), BaseValueType()));

    public IType ToDictionaryType() => CodeModelFactory.QuickType("Dictionary", new[] { BaseKeyType(), BaseValueType() });
    public IType ToDictionaryInterfaceType() => CodeModelFactory.QuickType("IDictionary", new[] { BaseKeyType(), BaseValueType() });

    public AbstractProperty ToNamedValue(string? name = null)
        => AbstractCodeModelParsing.AbstractProperty(ToDictionaryType(), name ?? Name ?? throw new ArgumentException($"No name for property"),
            ToDictionary2(), modifier: Modifier.Readonly | Modifier.Public, interfaceType: ToDictionaryInterfaceType());

    public IType BaseKeyType() => Keys.BaseType();
    public TypeSyntax BaseKeyTypeSyntax() => BaseKeyType().Syntax();
    public IType BaseValueType() => _valueType ?? TypeUtil.FindCommonType(KeyVaulePairs.Select(x => x.BaseType()));
    public TypeSyntax BaseValueTypeSyntax() => BaseValueType().Syntax();

}


