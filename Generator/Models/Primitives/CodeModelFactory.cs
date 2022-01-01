﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Parsing;
using Common.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public static class CodeModelFactory
    {
        public static List<T> List<T>(IEnumerable<T>? methods) => methods?.ToList() ?? new List<T>();

        public static Namespace Namespace(string name) => new(name);
        public static Namespace Namespace(Type type) => new(type.Namespace);

        public static QuickType Type(string name, bool required = true, bool isMulti = false, TypeSyntax? syntax = null, Type? type = null)
            => new(name, required, isMulti, syntax, type);

        public static IMethodHolder MetodHolder(Type type) => type switch
        {
            { IsInterface: true } => Interface(type),
            { IsEnum: true } => Enum(type),
            _ when ReflectionUtil.IsStatic(type) => StaticClass(type),
            _ => InstanceClass(type)
        };

        public static StaticClassFromReflection StaticClass(Type type) => new(type);
        public static InstanceClassFromReflection InstanceClass(Type type) => new(type);
        public static InterfaceFromReflection Interface(Type type) => new(type);
        public static EnumFromReflection Enum(Type type) => new(type);

        public static List<IMethod> Methods(Type type) => type.GetMethods().Select(x => new MethodFromReflection(x)).ToList<IMethod>();

        public static LiteralExpression Literal(object value) => new(value);
        public static List<LiteralExpression> Literals(IEnumerable<object> values) => values.Select(Literal).ToList();

        public static Property Property(IType type, string? name, Expression? value = null, Modifier? modifier = null) => new(type, name, value, modifier);
        public static Property Property(IType type, string name, ExpressionSyntax expression, Modifier? modifier = null) => new(type, name, new ExpressionFromSyntax(expression), modifier);
        public static Property Property(IType type, string name, string qualifiedName, Modifier? modifier = null) => new(type, name, new ExpressionFromSyntax(qualifiedName), modifier);
        public static Property Property(string name) => new(TypeShorthands.NullType, name);
        public static Property Property(ArgumentSyntax argument) => argument.Expression switch
        {
            TypeSyntax type => new(AbstractType.Parse(type), argument.NameColon?.Name.ToString()),
            DeclarationExpressionSyntax declaration => Property(declaration),
            _ => throw new ArgumentException($"Can't parse {nameof(Models.Property)} from '{argument}'.")
        };
        public static Property Property(DeclarationExpressionSyntax declaration) => new(AbstractType.Parse(declaration.Type), declaration.Designation switch
        {
            null => default,
            SingleVariableDesignationSyntax designation => designation.Identifier.ToString(),
            _ => throw new ArgumentException($"Can't parse {nameof(Models.Property)} from '{declaration}'.")
        });

        public static PropertyCollection PropertyCollection(PropertyCollection? collection) => collection ?? new();
        public static PropertyCollection PropertyCollection(IEnumerable<Property> properties, string? name = null) => new(properties, name);
        public static PropertyCollection PropertyCollection(Type type) => new(type);
        public static PropertyCollection PropertyCollection(string code) => code.Parse(code).Members.FirstOrDefault() switch
        {
            ClassDeclarationSyntax declaration => new PropertyCollection(declaration),
            RecordDeclarationSyntax declaration => new PropertyCollection(declaration),
            GlobalStatementSyntax statement => PropertyCollection(statement),
            _ => throw new ArgumentException($"Can't parse {nameof(Models.PropertyCollection)} from '{code}'.")
        };
        public static PropertyCollection PropertyCollection(GlobalStatementSyntax statement) => statement.Statement switch
        {
            ExpressionStatementSyntax expression => PropertyCollection(expression.Expression),
            _ => throw new ArgumentException($"Can't parse {nameof(Models.PropertyCollection)} from '{statement}'.")
        };
        public static PropertyCollection PropertyCollection(ExpressionSyntax expression) => expression switch
        {
            TupleExpressionSyntax declaration => PropertyCollection(declaration.Arguments),
            TupleTypeSyntax declaration => new PropertyCollection(declaration),
            _ => throw new ArgumentException($"Can't parse {nameof(Models.PropertyCollection)} from '{expression}'.")
        };
        public static PropertyCollection PropertyCollection(IEnumerable<ArgumentSyntax> arguments) => new(arguments.Select(x => Property(x)));

    }
}
