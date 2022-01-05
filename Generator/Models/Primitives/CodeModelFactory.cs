﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Parsing;
using Common.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public static class CodeModelFactory
    {
        public static readonly LiteralExpression NullValue = new(TypeShorthands.NullType);

        public static List<T> List<T>(IEnumerable<T>? objects) => objects?.ToList() ?? new List<T>();
        public static List<T> List<T>(params T[] objects) => objects?.ToList() ?? new List<T>();

        public static Namespace Namespace(string name) => new(name);
        public static Namespace Namespace(Type type) => new(type.Namespace);

        public static TypeFromReflection Type(Type type) => new(type);
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

        public static Property Property(IType type, string? name, IExpression? value = null, Modifier? modifier = null) => new(type, name, value, modifier);
        public static Property Property(IType type, string name, ExpressionSyntax expression, Modifier? modifier = null) => new(type, name, new ExpressionFromSyntax(expression), modifier);
        public static Property Property(IType type, string name, string qualifiedName, Modifier? modifier = null) => new(type, name, new ExpressionFromSyntax(qualifiedName), modifier);
        public static Property Property(string name) => new(TypeShorthands.NullType, name);
        public static Property Property(ArgumentSyntax argument) => argument.Expression switch
        {
            TypeSyntax type => new(Type(type), argument.NameColon?.Name.ToString()),
            DeclarationExpressionSyntax declaration => Property(declaration),
            _ => throw new ArgumentException($"Can't parse {nameof(Models.Property)} from '{argument}'.")
        };
        public static Property Property(DeclarationExpressionSyntax declaration) => new(Type(declaration.Type), declaration.Designation switch
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
        public static IExpression Expression(ExpressionSyntax? syntax) => syntax is null ? NullValue : new ExpressionFromSyntax(syntax);

        public static IType Type(string code) => Type(ParseTypeName(code));

        // TODO
        public static IType Type(TypeSyntax? type, bool required = true, TypeSyntax? fullType = null) => type switch
        {
            PredefinedTypeSyntax t => new QuickType(t.Keyword.ToString(), required, Syntax: fullType ?? t),
            NullableTypeSyntax t => Type(t.ElementType, false, fullType: fullType ?? t),
            IdentifierNameSyntax t => new QuickType(t.Identifier.ToString(), Syntax: fullType ?? t),
            ArrayTypeSyntax t => new QuickType(t.ElementType.ToString(), IsMulti: true, Syntax: fullType ?? t),
            GenericNameSyntax t => new QuickType(t.Identifier.ToString(), Syntax: fullType ?? t),
            null => TypeShorthands.NullType,
            _ => throw new ArgumentException($"Unhandled {nameof(TypeSyntax)}: '{type}'.")
        };

        public static Block Block(IEnumerable<IStatement> statements) => new(List(statements));
        public static Block Block(params IStatement[] statements) => new(List(statements));
        public static Block Block(IStatement statement) => statement is Block ? (statement as Block)! : new(List(statement));
        public static IfStatement If(IExpression condition, IStatement statement, IStatement? @else = null) => new(condition, statement, @else);
        public static MultiIfStatement MultiIf(IEnumerable<IfStatement> ifs, IStatement? @else = null) => new(List(ifs), @else);
        public static ForStatement For(VariableDeclaration declaration, IExpression condition, IExpression initializer, IExpression incrementors, IStatement statement)
            => new(declaration, initializer, condition, incrementors, statement);
        public static ForStatement For(VariableDeclaration declaration, IExpression condition, IExpression incrementors, IStatement statement)
            => new(declaration, null, condition, incrementors, statement);
        public static SimpleForStatement For(string Variable, IExpression Limit, IStatement Statement)
            => new(Variable, Limit, Block(Statement));
        public static VariableDeclaration Declaration(IType type, string name, IExpression value) => new(type, name, value);
        public static LocalDeclarationStatement LocalDeclaration(VariableDeclaration declaration, Modifier modifiers = Modifier.None) => new(declaration, modifiers);

        public static IdentifierExpression Identifier(string name, IType? type = null) => new(name, type);

        public static UnaryExpression UnaryExpression(IExpression input, OperationType operation, IType? type = null)
           => operation.IsUnaryOperator() ? new(input, type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not a unary operator: '{operation}'");

        public static BinaryExpression BinaryExpression(IExpression lhs, OperationType operation, IExpression rhs, IType? type = null)
            => operation.IsBinaryOperator() ? new(lhs, rhs, type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not a binary operator: '{operation}'");

        public static TernaryExpression TernaryExpression(IExpression input, IExpression output1, IExpression output2, IType? type = null, OperationType operation = OperationType.Ternary)
            => operation.IsTernaryOperator() ? new(input, output1, output2, type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not a ternary operator: '{operation}'");

        public static AnyArgExpression AnyArgExpression(IEnumerable<IExpression>? inputs, OperationType operation, IType? type = null)
            => operation.IsAnyArgOperator() ? new(List(inputs), type, operation) : throw new ArgumentException($"Not an any arg operator: '{operation}'");


    }
}
