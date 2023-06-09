﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.AbstractCodeModels.Member;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.AbstractCodeModelParsing;

namespace CodeModels.Factory;

public static class AbstractCodeModelFactory
{
    private static List<T> List<T>(IEnumerable<T>? objects) => objects?.ToList() ?? new List<T>();
    private static List<T> List<T>(params T[] objects) => objects?.ToList() ?? new List<T>();

    public static AbstractProperty FieldNamedValue(string? name, IExpression value, Modifier modifier = Modifier.None) => NamedValue(value.Get_Type(), name, value, Modifier.Field.SetFlags(modifier));
    public static AbstractProperty NamedValue(INamedValue value) => value is AbstractProperty a ? a : new(value.Type ?? value.Value.Get_Type() ?? TypeShorthands.NullType, value.Name, value.Value, value.Modifier);
    public static AbstractProperty NamedValue(IType? type = null, string? name = null, IExpression? value = null, Modifier modifier = Modifier.None) => new(type ?? value?.Get_Type() ?? TypeShorthands.NullType, name, value, modifier);
    public static AbstractProperty NamedValue(string name, IExpression value, Modifier modifier = Modifier.None) => NamedValue(null, name, value, modifier);
    public static AbstractProperty NamedValue(IType type, string name, string qualifiedName, Modifier modifier = Modifier.None) => NamedValue(type, name, CodeModelFactory.IdentifierExp(qualifiedName), modifier);
    public static AbstractProperty NamedValue<T>(string? name, IExpression? value = null, Modifier modifier = Modifier.None) => NamedValue(CodeModelFactory.Type<T>(), name, value, modifier);
    public static AbstractProperty NamedValue<T>(string name, string qualifiedName, Modifier modifier = Modifier.None) => NamedValue(CodeModelFactory.Type<T>(), name, CodeModelFactory.IdentifierExp(qualifiedName), modifier);
    public static AbstractProperty NamedValue(string name) => NamedValue(null, name);


    public static NamedValueCollection NamedValues(NamedValueCollection? collection) => collection ?? new();
    public static NamedValueCollection NamedValues(IEnumerable<INamedValue> properties, string? name = null) => new(properties.Select(x => NamedValue(x)), name);
    public static NamedValueCollection NamedValues(string name, params AbstractProperty[] properties) => new(properties, name);
    public static NamedValueCollection NamedValues(params AbstractProperty[] properties) => new(properties);
    public static NamedValueCollection NamedValues(Type type) => CodeModelsFromReflection.NamedValues(type);
    public static NamedValueCollection NamedValues(string code) => ParseNamedValues(code);
    public static NamedValueCollection EmptyNamedValues(string name) => NamedValues(name, properties: Array.Empty<AbstractProperty>());

    public static ExpressionCollection Expressions(IEnumerable<IExpression>? values = null, IType? specifiedType = null)
        => ExpressionCollection.Create(values, specifiedType);
    public static ExpressionCollection Expressions(params IExpression[] values)
        => ExpressionCollection.Create(values);
    public static ExpressionCollection Expressions(string commaSeparatedValues)
        => Expressions(commaSeparatedValues.Trim().Split(',').Select(CodeModelFactory.Literal));
    public static ExpressionCollection Expressions(EnumDeclarationSyntax declaration)
        => Expressions(declaration.Members.Select(x => CodeModelFactory.Literal(x.Identifier.ToString())));

    public static ExpressionMap ExpressionMap(IEnumerable<ExpressionsMap> keyVaulePairs, ExpressionCollection keys, ExpressionCollection values, string? name = null)
        => AbstractCodeModels.Collection.ExpressionMap.Create(keyVaulePairs, keys, values, name);
    public static ExpressionMap ExpressionMap(IEnumerable<ExpressionsMap> values, string? name = null, AbstractType? valueType = null)
        => AbstractCodeModels.Collection.ExpressionMap.Create(values, name, valueType);

    public static ExpressionsMap ExpressionsMap(IExpression key, IEnumerable<IExpression> values, bool multiValues = false, IType? valueType = null)
        => AbstractCodeModels.Collection.ExpressionsMap.Create(key, values, multiValues, valueType);
    public static ExpressionsMap ExpressionsMap(IExpression key, IExpression value, bool multiValues = false, AbstractType? valueType = null)
        => AbstractCodeModels.Collection.ExpressionsMap.Create(key, value, multiValues, valueType);
    public static ExpressionsMap ExpressionsMap(IExpression key, string commaSeparatedValues)
        => AbstractCodeModels.Collection.ExpressionsMap.Create(key, commaSeparatedValues);

    public static StaticClass StaticClass(string identifier, NamedValueCollection? properties = null, IEnumerable<IMethod>? methods = null, Namespace? @namespace = null, Modifier topLevelModifier = Modifier.None, Modifier memberModifier = Modifier.None)
        => new(identifier, NamedValues(properties), List(methods), @namespace, topLevelModifier, memberModifier);
    public static InstanceClass InstanceClass(string identifier, NamedValueCollection? properties = null, IEnumerable<IMethod>? methods = null, Namespace? @namespace = null)
            => new(identifier, properties, methods, @namespace);

    public static IClassDeclaration Class(NamedValueCollection collection) => collection.ToClassModel();

    public static AbstractPropertyExpression Expression(AbstractProperty property, IExpression? instance = null)
        => AbstractPropertyExpression.Create(property, instance);
}
