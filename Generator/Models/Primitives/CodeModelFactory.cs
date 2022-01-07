using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Parsing;
using Common.Reflection;
using Common.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        public static IType Type(string code) => Type(ParseTypeName(code));
        public static IType Type(IdentifierExpression identifier) => Type(identifier.ToString());
        public static IType Type(SyntaxToken token) => Type(token.ToString());

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

        public static IMethodHolder MetodHolder(Type type) => type switch
        {
            { IsInterface: true } => Interface(type),
            { IsEnum: true } => Enum(type),
            _ when ReflectionUtil.IsStatic(type) => StaticClass(type),
            _ => InstanceClass(type)
        };

        public static StaticClassFromReflection StaticClass(Type type) => new(type);
        public static StaticClass StaticClass(string identifier, PropertyCollection? properties = null, IEnumerable<IMethod>? methods = null, Namespace? @namespace = null, Modifier topLevelModifier = Modifier.None, Modifier memberModifier = Modifier.None)
            => new(identifier, PropertyCollection(properties), List(methods), @namespace, topLevelModifier, memberModifier);
        public static InstanceClassFromReflection InstanceClass(Type type) => new(type);
        public static InterfaceFromReflection Interface(Type type) => new(type);
        public static EnumFromReflection Enum(Type type) => new(type);

        public static List<IMethod> Methods(Type type) => type.GetMethods().Select(x => new MethodFromReflection(x)).ToList<IMethod>();

        public static LiteralExpression Literal(object value) => new(value);
        public static IExpression Value(object? value) => value switch
        {
            null => NullValue,
            Array array => Values(array),
            _ => Literal(value),
        };
        public static ExpressionCollection Values(IEnumerable<object?> values) => new(values.Select(Value));
        public static ExpressionCollection Values(Array values) => new(CollectionUtil.ModernizeArray(values).Select(Value));
        public static ExpressionCollection Values(params object?[] values) => new(values.Select(Value));
        public static List<LiteralExpression> Literals(IEnumerable<object> values) => values.Select(Literal).ToList();

        public static Property Field(string? name, IExpression value, Modifier modifier = Modifier.None) => Property(value.Type, name, value, Modifier.Field.SetFlags(modifier));
        public static Property Property(IType type, string? name, IExpression? value = null, Modifier modifier = Modifier.None) => new(type, name, value, modifier);
        public static Property Property(IType type, string name, ExpressionSyntax expression, Modifier modifier = Modifier.None) => new(type, name, new ExpressionFromSyntax(expression), modifier);
        public static Property Property(IType type, string name, string qualifiedName, Modifier modifier = Modifier.None) => new(type, name, new ExpressionFromSyntax(qualifiedName), modifier);
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
        public static PropertyCollection PropertyCollection(string name, params Property[] properties) => new(properties, name);
        public static PropertyCollection PropertyCollection(params Property[] properties) => new(properties);
        public static PropertyCollection PropertyCollection(Type type) => new(type);
        public static PropertyCollection PropertyCollection(string code) => code.Parse(code).Members.FirstOrDefault() switch
        {
            ClassDeclarationSyntax declaration => new(declaration),
            RecordDeclarationSyntax declaration => new(declaration),
            GlobalStatementSyntax statement => PropertyCollection(statement),
            _ => throw new ArgumentException($"Can't parse {nameof(Models.PropertyCollection)} from '{code}'.")
        };
        public static PropertyCollection PropertyCollection(GlobalStatementSyntax statement) => statement.Statement switch
        {
            ExpressionStatementSyntax expression => PropertyCollection(expression.Expression),
            _ => throw new ArgumentException($"Can't parse {nameof(Models.PropertyCollection)} from '{statement}'.")
        };
        public static PropertyCollection PropertyCollection(ExpressionSyntax syntax) => syntax switch
        {
            TupleExpressionSyntax declaration => PropertyCollection(declaration.Arguments),
            TupleTypeSyntax declaration => new PropertyCollection(declaration),
            _ => throw new ArgumentException($"Can't parse {nameof(Models.PropertyCollection)} from '{syntax}'.")
        };
        public static PropertyCollection PropertyCollection(IEnumerable<ArgumentSyntax> arguments) => new(arguments.Select(x => Property(x)));
        public static IExpression Expression(ExpressionSyntax? syntax) => syntax is null ? NullValue : new ExpressionFromSyntax(syntax);

        public static IStatement Statement(StatementSyntax syntax) => syntax switch
        {

            _ => throw new ArgumentException($"Can't parse {nameof(IStatement)} from '{syntax}'.")
        };
        public static List<IStatement> Statements(params IStatement[] statements) => statements.ToList();

        public static Method Method(string name, PropertyCollection parameters, IType returnType, Block body, Modifier modifier = Modifier.Public)
            => new(name, parameters, returnType, body, modifier);
        public static Method Method(string name, PropertyCollection parameters, IType returnType, List<IStatement> statements, Modifier modifier = Modifier.Public)
            => new(name, parameters, returnType, Block(statements), modifier);
        public static Method Method(string name, PropertyCollection parameters, IType returnType, IExpression expressionBody, Modifier modifier = Modifier.Public)
            => new(name, parameters, returnType, expressionBody, modifier);
        public static Method Method(MethodDeclarationSyntax method)
            => new(method.GetName(), new PropertyCollection(method), Type(method.ReturnType), method.Body is null ? null : Block(method.Body), method.ExpressionBody is null ? null : Expression(method.ExpressionBody.Expression));


        public static Constructor Constructor(string name, PropertyCollection parameters, Block body, Modifier modifier = Modifier.Public)
            => new(name, parameters, body, modifier);
        public static Constructor Constructor(string name, PropertyCollection parameters, List<IStatement> statements, Modifier modifier = Modifier.Public)
            => new(name, parameters, Block(statements), modifier);
        public static Constructor Constructor(string name, PropertyCollection parameters, IExpression expressionBody, Modifier modifier = Modifier.Public)
            => new(name, parameters, expressionBody, modifier);
        public static Constructor Constructor(ConstructorDeclarationSyntax constructor)
            => new(constructor.Identifier.ToString(), new PropertyCollection(constructor), constructor.Body is null ? null : Block(constructor.Body), constructor.ExpressionBody is null ? null : Expression(constructor.ExpressionBody.Expression));

        public static Block Block(IEnumerable<IStatement> statements) => new(List(statements));
        public static Block Block(params IStatement[] statements) => new(List(statements));
        public static Block Block(IStatement statement, bool condition = true) => !condition || statement is Block ? (statement as Block)! : new(List(statement));
        public static Block Block(BlockSyntax syntax) => syntax switch
        {
            // TODO
            _ => throw new ArgumentException($"Can't parse {nameof(IStatement)} from '{syntax}'.")
        };

        public static IfStatement If(IExpression condition, IStatement statement, IStatement? @else = null) => new(condition, statement, @else);
        public static MultiIfStatement MultiIf(IEnumerable<IfStatement> ifs, IStatement? @else = null) => new(List(ifs), @else);

        public static ForStatement For(VariableDeclaration declaration, IExpression? initializer, IExpression condition, IExpression incrementors, IStatement statement, bool blockify = true)
            => new(declaration, initializer, condition, incrementors, Block(statement, blockify));
        public static ForStatement For(VariableDeclaration declaration, IExpression condition, IExpression incrementors, IStatement statement, bool blockify = true)
            => For(declaration, null, condition, incrementors, statement, blockify);
        public static SimpleForStatement For(string variable, IExpression limit, IStatement statement, bool blockify = true)
            => new(variable, limit, Block(statement, blockify));

        public static ForEachStatement ForEach(IType? type, string identifier, IExpression expression, IStatement statement, bool blockify = true)
            => new(type, identifier, expression, Block(statement, blockify));
        public static ForEachStatement ForEach(string identifier, IExpression expression, IStatement statement, bool blockify = true)
            => new(identifier, expression, Block(statement, blockify));

        public static WhileStatement While(IExpression condition, IStatement statement, bool blockify = true) => new(condition, Block(statement, blockify));

        public static DoStatement Do(IStatement statement, IExpression condition, bool blockify = true) => new(Block(statement, blockify), condition);

        public static ReturnStatement Return(IExpression expression) => new(expression);
        public static ContinueStatement Continue() => new();
        public static BreakStatement Break() => new();

        public static VariableDeclaration Declaration(IType type, string name, IExpression value) => new(type, name, value);
        public static LocalDeclarationStatement LocalDeclaration(VariableDeclaration declaration, Modifier modifiers = Modifier.None) => new(declaration, modifiers);
        public static LocalDeclarationStatement LocalDeclaration(IType type, string name, IExpression? value = null, Modifier modifiers = Modifier.None)
            => LocalDeclaration(new(type, name, value), modifiers);

        public static TryStatement Try(IStatement statement, IEnumerable<CatchClause> catchClauses, FinallyClause? @finally = null)
            => new(statement, List(catchClauses), @finally);
        public static TryStatement Try(IStatement statement, CatchClause catchClause, FinallyClause? @finally = null)
            => new(statement, List(catchClause), @finally);
        public static CatchClause Catch(IType type, string identifier, IStatement statement) => new(type, identifier, statement);
        public static CatchClause Catch(IType type, IStatement statement) => new(type, null, statement);
        public static FinallyClause Finally(IStatement statement) => new(statement);
        public static ThrowStatement Throw(IExpression expression) => new(expression);
        public static ThrowExpression ThrowExpression(IExpression expression) => new(expression);

        public static SwitchStatement Switch(IExpression expression, IEnumerable<SwitchSection> sections, IStatement? @default = null)
            => @default is null ? new(expression, List(sections)) : new(expression, List(sections), @default);
        public static SwitchSection Case(IExpression label, IStatement statement) => new(label, statement);
        public static SwitchSection Cases(IEnumerable<IExpression> labels, IStatement statement) => new(labels.ToList(), statement);

        public static IdentifierExpression Identifier(string name, IType? type = null) => new(name, type);

        public static UnaryExpression UnaryExpression(IExpression input, OperationType operation, IType? type = null)
           => operation.IsUnaryOperator() ? new(input, type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not a unary operator: '{operation}'");

        public static BinaryExpression BinaryExpression(IExpression lhs, OperationType operation, IExpression rhs, IType? type = null)
            => operation.IsBinaryOperator() ? new(lhs, rhs, type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not a binary operator: '{operation}'");

        public static TernaryExpression TernaryExpression(IExpression input, IExpression output1, IExpression output2, IType? type = null, OperationType operation = OperationType.Ternary)
            => operation.IsTernaryOperator() ? new(input, output1, output2, type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not a ternary operator: '{operation}'");

        public static AnyArgExpression AnyArgExpression(IEnumerable<IExpression>? inputs, OperationType operation, IType? type = null)
            => operation.IsAnyArgOperator() ? new(List(inputs), type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not an any arg operator: '{operation}'");


    }
}
