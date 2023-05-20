using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.AbstractCodeModels.Member;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
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
    public static AbstractProperty NamedValue(IType type, string name, ExpressionSyntax expression, Modifier modifier = Modifier.None) => NamedValue(type, name, CodeModelFactory.Expression(expression), modifier);
    public static AbstractProperty NamedValue(IType type, string name, string qualifiedName, Modifier modifier = Modifier.None) => NamedValue(type, name, CodeModelFactory.ExpressionFromQualifiedName(qualifiedName), modifier);
    public static AbstractProperty NamedValue<T>(string? name, IExpression? value = null, Modifier modifier = Modifier.None) => NamedValue(CodeModelFactory.Type<T>(), name, value, modifier);
    public static AbstractProperty NamedValue<T>(string name, ExpressionSyntax expression, Modifier modifier = Modifier.None) => NamedValue(CodeModelFactory.Type<T>(), name, CodeModelFactory.Expression(expression), modifier);
    public static AbstractProperty NamedValue<T>(string name, string qualifiedName, Modifier modifier = Modifier.None) => NamedValue(CodeModelFactory.Type<T>(), name, CodeModelFactory.ExpressionFromQualifiedName(qualifiedName), modifier);
    public static AbstractProperty NamedValue(string name) => NamedValue(null, name);
    public static AbstractProperty NamedValue(ArgumentSyntax argument) => ParseProperty(argument);
    public static AbstractProperty NamedValue(DeclarationExpressionSyntax declaration) => ParseProperty(declaration);


    public static NamedValueCollection NamedValues(NamedValueCollection? collection) => collection ?? new();
    public static NamedValueCollection NamedValues(IEnumerable<INamedValue> properties, string? name = null) => new(properties.Select(x => NamedValue(x)), name);
    public static NamedValueCollection NamedValues(string name, params AbstractProperty[] properties) => new(properties, name);
    public static NamedValueCollection NamedValues(params AbstractProperty[] properties) => new(properties);
    public static NamedValueCollection NamedValues(Type type) => CodeModelsFromReflection.NamedValues(type);
    public static NamedValueCollection NamedValues(string code) => ParseNamedValues(code);
    public static NamedValueCollection EmptyNamedValues(string name) => NamedValues(name, properties: Array.Empty<AbstractProperty>());



    public static StaticClass StaticClass(string identifier, NamedValueCollection? properties = null, IEnumerable<IMethod>? methods = null, Namespace? @namespace = null, Modifier topLevelModifier = Modifier.None, Modifier memberModifier = Modifier.None)
        => new(identifier, NamedValues(properties), List(methods), @namespace, topLevelModifier, memberModifier);
    public static InstanceClass InstanceClass(string identifier, NamedValueCollection? properties = null, IEnumerable<IMethod>? methods = null, Namespace? @namespace = null)
            => new(identifier, properties, methods, @namespace);

    public static IClassDeclaration Class(NamedValueCollection collection) => collection.ToClassModel();


}
