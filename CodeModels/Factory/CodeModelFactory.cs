using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeModels.Execution.Scope;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Access;
using CodeModels.Models.Primitives.Expression.Invocation;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Reflection;
using CodeModels.Utils;
using Common.Extensions;
using Common.Reflection;
using Common.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelParsing;

namespace CodeModels.Factory;

public static class CodeModelFactory
{
    public static readonly LiteralExpression VoidValue = new(TypeShorthands.VoidType);
    public static readonly LiteralExpression NullValue = new(TypeShorthands.NullType);
    public static readonly LiteralExpression DefaultValue = new(TypeShorthands.NullType);
    public static readonly ClassDeclaration VoidClass = Class("_VoidClass");

    public static List<T> List<T>(IEnumerable<T>? objects) => objects?.ToList() ?? new List<T>();
    public static List<T> List<T>(params T[] objects) => objects?.ToList() ?? new List<T>();

    public static Namespace Namespace(string name) => new(name);
    public static Namespace Namespace(Type type) => CodeModelsFromReflection.Namespace(type);

    public static TypeFromReflection Type(Type type) => CodeModelsFromReflection.Type(type);
    public static TypeFromReflection Type<T>() => Type(typeof(T));
    public static QuickType Type(string name, bool required = true, bool isMulti = false, Type? type = null)
        => QuickType(name, required, isMulti, type);
    public static QuickType Type(IType type, bool? required = null, bool? isMulti = null)
        => QuickType(type.TypeName, required ?? type.Required, isMulti ?? type.IsMulti);
    public static IType Type(IdentifierExpression identifier, SemanticModel? model = null) => ParseType(identifier.ToString(), model);
    public static IType Type(string code) => ParseType(code);
    public static IType Type(ITypeDeclaration declaration) => QuickType(declaration.Name, declaration: declaration);
    public static IType Type(SyntaxToken token, SemanticModel? model = null) => Parse(token, model);
    public static IType Type(TypeSyntax? type, bool required = true) => ParseType(type, required);
    public static IType Type(Microsoft.CodeAnalysis.TypeInfo typeInfo) => Type(typeInfo.Type!);
    public static IType Type(ITypeSymbol symbol) => Type(SemanticReflection.GetType(symbol));
    public static QuickType QuickType(string identifier,
        bool required = true,
        bool isMulti = false,
        Type? type = null,
        ITypeDeclaration? declaration = null,
        ITypeSymbol? symbol = null) => Models.QuickType.Create(identifier, required, isMulti, type, declaration, symbol);
    public static QuickType QuickType(string identifier,
        IEnumerable<IType> genericTypes,
        bool required = true,
        bool isMulti = false,
        Type? type = null,
        ITypeDeclaration? declaration = null,
        ITypeSymbol? symbol = null) => Models.QuickType.Create(identifier, genericTypes, required, isMulti, type, declaration, symbol);

    public static CompilationUnit CompilationUnit(List<IMember> members, List<UsingDirective>? usings = null, List<AttributeList>? attributes = null, List<ExternAliasDirective>? externs = null)
        => new(members, usings ?? new List<UsingDirective>(), attributes ?? new List<AttributeList>(), externs);

    public static IBaseTypeDeclaration MetodHolder(Type type) => type switch
    {
        { IsInterface: true } => Interface(type),
        { IsEnum: true } => Enum(type),
        _ when ReflectionUtil.IsStatic(type) => StaticClass(type),
        _ => InstanceClass(type)
    };
    public static Modifier Modifiers(SyntaxTokenList tokenList) => ParseModifier(tokenList);
    public static Modifier SingleModifier(SyntaxToken token) => ParseSingleModifier(token);

    public static StaticClassFromReflection StaticClass(Type type) => CodeModelsFromReflection.StaticClass(type);
    public static StaticClass StaticClass(string identifier, NamedValueCollection? properties = null, IEnumerable<IMethod>? methods = null, Namespace? @namespace = null, Modifier topLevelModifier = Modifier.None, Modifier memberModifier = Modifier.None)
        => new(identifier, NamedValues(properties), List(methods), @namespace, topLevelModifier, memberModifier);
    public static InstanceClassFromReflection InstanceClass(Type type) => CodeModelsFromReflection.InstanceClass(type);
    public static InstanceClass InstanceClass(string identifier, NamedValueCollection? properties = null, IEnumerable<IMethod>? methods = null, Namespace? @namespace = null)
        => new(identifier, properties, methods, @namespace);
    public static InterfaceFromReflection Interface(Type type) => CodeModelsFromReflection.Interface(type);
    public static EnumFromReflection Enum(Type type) => CodeModelsFromReflection.Enum(type);

    public static List<IMethod> Methods(Type type) => CodeModelsFromReflection.Methods(type);

    public static IExpression Literal(object? value) => value is InstantiatedObject o ? o : value is LiteralExpression l ? l : new LiteralExpression(value);
    public static IExpression Value(object? value) => value switch
    {
        null => NullValue,
        Array array => Values(array),
        _ => Literal(value),
    };
    public static ExpressionCollection Values(IEnumerable<object?> values) => new(values.Select(Value));
    public static ExpressionCollection Values(Array values) => new(CollectionUtil.ModernizeArray(values).Select(Value));
    public static ExpressionCollection Values(params object?[] values) => new(values.Select(Value));
    public static List<IExpression> Literals(IEnumerable<object> values) => values.Select(Literal).ToList();
    public static InvocationExpression Invocation(Method method, IExpression caller, IEnumerable<IExpression>? arguments = null, IEnumerable<ICodeModelExecutionScope>? scopes = null) => new(method, caller, List(arguments), List(scopes));
    public static ConstructorInvocationExpression ConstructorInvocation(Constructor constructor, IEnumerable<IExpression>? arguments = null) => new(constructor, List(arguments));
    //public static OperationCall OperationCall(Method method, IExpression caller, IEnumerable<IExpression>? arguments = null) => new(method, caller, List(arguments));
    public static MemberAccessExpression MemberAccess(FieldModel field, IExpression caller) => new(caller, Identifier(field.Name, model: field));

    public static Property FieldProperty(string? name, IExpression value, Modifier modifier = Modifier.None) => Property(value.Get_Type(), name, value, Modifier.Field.SetFlags(modifier));
    public static Property Property(IType? type = null, string? name = null, IExpression? value = null, Modifier modifier = Modifier.None) => new(type ?? value?.Get_Type() ?? TypeShorthands.NullType, name, value, modifier);
    public static Property Property(string name, IExpression value, Modifier modifier = Modifier.None) => Property(null, name, value, modifier);
    public static Property Property(IType type, string name, ExpressionSyntax expression, Modifier modifier = Modifier.None) => Property(type, name, Expression(expression), modifier);
    public static Property Property(IType type, string name, string qualifiedName, Modifier modifier = Modifier.None) => Property(type, name, ExpressionFromQualifiedName(qualifiedName), modifier);
    public static Property Property<T>(string? name, IExpression? value = null, Modifier modifier = Modifier.None) => Property(Type<T>(), name, value, modifier);
    public static Property Property<T>(string name, ExpressionSyntax expression, Modifier modifier = Modifier.None) => Property(Type<T>(), name, Expression(expression), modifier);
    public static Property Property<T>(string name, string qualifiedName, Modifier modifier = Modifier.None) => Property(Type<T>(), name, ExpressionFromQualifiedName(qualifiedName), modifier);
    public static Property Property(string name) => Property(null, name);
    public static Property Property(ArgumentSyntax argument) => ParseProperty(argument);
    public static Property Property(DeclarationExpressionSyntax declaration) => ParseProperty(declaration);

    public static FieldModel FieldModel(IType? type, string name, IExpression? value = null, Modifier modifier = Modifier.None)
        => Models.FieldModel.Create(name, type ?? value?.Get_Type() ?? TypeShorthands.NullType, modifier: modifier, value: value);
    public static FieldModel FieldModel(string name, IExpression value, Modifier modifier = Modifier.None)
        => FieldModel(value?.Get_Type(), name, modifier: modifier, value: value);
    public static FieldModel FieldModel<T>(string name, IExpression? value = null, Modifier modifier = Modifier.None)
            => FieldModel(Type<T>(), name, modifier: modifier, value: value);

    public static PropertyModel PropertyModel(IType? type, string name, IEnumerable<Accessor>? accessors = null, IExpression? value = null, Modifier modifier = Modifier.None)
        => Models.PropertyModel.Create(name, type ?? value?.Get_Type() ?? TypeShorthands.NullType, accessors ?? new Accessor[] { Accessor(AccessorType.Get), Accessor(AccessorType.Set) }, modifier: modifier, value: value);
    public static PropertyModel PropertyModel(string name, IExpression value, IEnumerable<Accessor>? accessors = null, Modifier modifier = Modifier.None)
        => PropertyModel(value?.Get_Type(), name, accessors, modifier: modifier, value: value);
    public static PropertyModel PropertyModel<T>(string name, IExpression? value = null, Modifier modifier = Modifier.None)
            => PropertyModel(Type<T>(), name, modifier: modifier, value: value);

    public static Accessor Accessor(AccessorType type,
    Block? body = null,
    IExpression? expressionBody = null,
    IEnumerable<AttributeList>? attributes = null,
    Modifier modifier = Modifier.None) => Models.Accessor.Create(type, body, expressionBody, attributes, modifier);

    public static PropertyModel PropertyModel(string name,
    IType type,
    List<AttributeList>? attributes = null,
    List<Accessor>? accessors = null,
    Modifier modifier = Modifier.None,
    IExpression? value = null) => new(
        Name: name,
        Type: type,
        Attributes: attributes ?? List<AttributeList>(),
        Accessors: accessors ?? new List<Accessor>(),
        Modifier: modifier,
        Value: value ?? VoidValue);

    public static NamedValueCollection NamedValues(NamedValueCollection? collection) => collection ?? new();
    public static NamedValueCollection NamedValues(IEnumerable<Property> properties, string? name = null) => new(properties, name);
    public static NamedValueCollection NamedValues(string name, params Property[] properties) => new(properties, name);
    public static NamedValueCollection NamedValues(params Property[] properties) => new(properties);
    public static NamedValueCollection NamedValues(Type type) => CodeModelsFromReflection.NamedValues(type);
    public static NamedValueCollection NamedValues(string code) => ParseNamedValues(code);
    public static NamedValueCollection NamedValues(GlobalStatementSyntax statement) => ParseNamedValues(statement);
    public static NamedValueCollection NamedValues(ExpressionSyntax syntax) => ParseNamedValues(syntax);

    public static IdentifierExpression ExpressionFromQualifiedName(string qualifiedName) => new(qualifiedName);
    public static IExpression Expression(ExpressionSyntax? syntax, IType? Type = null) => ParseExpression(syntax, Type);
    public static IStatement Statement(StatementSyntax syntax) => Parse(syntax);
    public static List<IStatement> Statements(params IStatement[] statements) => statements.ToList();

    public static Method Method(string name, NamedValueCollection parameters, IType returnType, Block body, Modifier modifier = Modifier.Public)
        => new(name, parameters, returnType, body, modifier);
    public static Method Method(string name, NamedValueCollection parameters, IType returnType, List<IStatement> statements, Modifier modifier = Modifier.Public)
        => new(name, parameters, returnType, Block(statements), modifier);
    public static Method Method(string name, NamedValueCollection parameters, IType returnType, IExpression expressionBody, Modifier modifier = Modifier.Public)
        => new(name, parameters, returnType, expressionBody, modifier);
    public static Method Method(string name, IType returnType, Block body, Modifier modifier = Modifier.Public)
        => Method(name, NamedValues(), returnType, body, modifier);
    public static Method Method(string name, IType returnType, List<IStatement> statements, Modifier modifier = Modifier.Public)
        => Method(name, NamedValues(), returnType, Block(statements), modifier);
    public static Method Method(string name, IType returnType, IExpression expressionBody, Modifier modifier = Modifier.Public)
        => Method(name, NamedValues(), returnType, expressionBody, modifier);
    public static ICodeModel Member(MemberInfo member) => member switch
    {
        Type type => new TypeFromReflection(type),
        //FieldInfo field => Property(field),
        MethodBase @base => Method(@base),
        _ => throw new NotImplementedException($"Unhandled member: {member}")
    };
    public static ICodeModel Method(MethodBase @base) => @base switch
    {
        //ConstructorInfo constructor => new ConstructorFromReflection(constructor),
        MethodInfo method => new MethodFromReflection(method),
        _ => throw new NotImplementedException($"Unhandled base: {@base}")
    };

    public static Method Method(MethodDeclarationSyntax method, SemanticModel? model = null) => Parse(method, model);

    public static Constructor ConstructorFull(ITypeDeclaration type, NamedValueCollection parameters, Block? body = null, IExpression? expressionBody = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => Models.Constructor.Create(type, parameters, body is null && expressionBody is null ? Block() : body, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(IType type, NamedValueCollection parameters, Block? body = null, IExpression? expressionBody = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => Models.Constructor.Create(type, parameters, body is null && expressionBody is null ? Block() : body, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(ITypeDeclaration type, NamedValueCollection parameters, IExpression expressionBody,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(type, parameters, null, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(ITypeDeclaration type, NamedValueCollection parameters, List<IStatement> statements, Modifier modifier = Modifier.Public)
        => ConstructorFull(type, parameters, Block(statements), null, modifier);
    public static Constructor Constructor(string type, NamedValueCollection parameters, Block? body = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(Class(type), parameters, body, null, Modifier, Attributes);
    public static Constructor Constructor(string type, NamedValueCollection parameters, IExpression? expressionBody = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(Class(type), parameters, null, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(ITypeDeclaration type, Block? body = null, IExpression? expressionBody = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(type, NamedValues(), body, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(ITypeDeclaration type, IExpression expressionBody,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(type, NamedValues(), null, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(ITypeDeclaration type, List<IStatement> statements, Modifier modifier = Modifier.Public)
        => ConstructorFull(type, NamedValues(), Block(statements), null, modifier);
    public static Constructor Constructor(string type, Block? body = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(Class(type), NamedValues(), body, null, Modifier, Attributes);
    public static Constructor Constructor(string type, IExpression expressionBody,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(Class(type), NamedValues(), null, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(NamedValueCollection parameters, IExpression expressionBody,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(VoidClass, parameters, null, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(NamedValueCollection parameters, List<IStatement> statements, Modifier modifier = Modifier.Public)
        => ConstructorFull(VoidClass, parameters, Block(statements), null, modifier);
    public static Constructor Constructor(NamedValueCollection parameters, Block? body = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(VoidClass, parameters, body, null, Modifier, Attributes);
    public static Constructor Constructor(IExpression expressionBody,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(VoidClass, NamedValues(), null, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(List<IStatement> statements, Modifier modifier = Modifier.Public)
        => ConstructorFull(VoidClass, NamedValues(), Block(statements), null, modifier);
    public static Constructor Constructor(Block? body = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(VoidClass, NamedValues(), body, null, Modifier, Attributes);
    public static IConstructor Constructor(ConstructorDeclarationSyntax syntax) => Parse(syntax);

    public static Block Block(IEnumerable<IStatement> statements) => new(List(statements));
    public static Block Block(params IStatement[] statements) => new(List(statements));
    public static Block Block(IStatement statement, bool condition = true) => !condition || statement is Block ? (statement as Block)! : new(List(statement));
    public static Block Block(IExpression expression) => Block(new[] { Statement(expression) });
    public static Block Block(BlockSyntax syntax) => Parse(syntax);

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

    public static SwitchStatement Switch(IExpression expression, IEnumerable<SwitchSection> sections, IStatement? @default = null)
         => @default is null ? new(expression, List(sections)) : new(expression, List(sections), @default);
    public static SwitchSection Case(IExpression label, IStatement statement) => new(label, statement);
    public static SwitchSection Cases(IEnumerable<IExpression> labels, IStatement statement) => new(labels.ToList(), List(statement));

    public static ThisExpression This() => new(TypeShorthands.VoidType);
    public static ReturnStatement Return(IExpression expression) => new(expression);
    public static ReturnStatement Return(object? literal) => new(Literal(literal));
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

    public static IdentifierExpression Identifier(string name, IType? type = null, ISymbol? symbol = null, ICodeModel? model = null) => new(name, type, symbol, model);

    public static UnaryExpression UnaryExpression(IExpression input, OperationType operation, IType? type = null)
       => operation.IsUnaryOperator() ? new(input, type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not a unary operator: '{operation}'");

    public static BinaryExpression BinaryExpression(IExpression lhs, OperationType operation, IExpression rhs, IType? type = null)
        => operation.IsBinaryOperator() ? new(lhs, rhs, type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not a binary operator: '{operation}'");

    public static PatternExpression PatternExpression(IPattern pattern, IExpression rhs, IType? type = null)
        => new(pattern, rhs, type ?? TypeShorthands.NullType);

    public static MemberAccessExpression MemberAccess(IExpression lhs, IdentifierExpression property, IType? type = null)
        => new(lhs, property, type);

    public static TernaryExpression TernaryExpression(IExpression input, IExpression output1, IExpression output2, IType? type = null, OperationType operation = OperationType.Ternary)
        => operation.IsTernaryOperator() ? new(input, output1, output2, type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not a ternary operator: '{operation}'");

    public static AnyArgExpression<ExpressionSyntax> AnyArgExpression(IEnumerable<IExpression>? inputs, OperationType operation, IType? type = null)
        => operation.IsAnyArgOperator() ? new(List(inputs), type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not an any arg operator: '{operation}'");

    public static AssignmentExpression Assignment(IExpression left, SyntaxKind kind, IExpression right) => new(left, right, kind);
    public static AssignmentExpression Assignment(IExpression left, IExpression right) => new(left, right, SyntaxKind.SimpleAssignmentExpression);

    public static ExpressionStatement Statement(IExpression expression) => new(expression);

    public static ClassDeclaration Class(string name,
        IEnumerable<IMember>? members = null,
        Namespace? @namespace = null,
        Modifier? modifier = null) => ClassDeclaration.Create(NamespaceUtils.NamePart(name), members, @namespace is null && NamespaceUtils.IsMemberAccess(name) ? Namespace(NamespaceUtils.PathPart(name)) : @namespace, modifier);
    public static ClassDeclaration Class(string name, params IMember[] membersArray) => Class(name, members: membersArray);
    public static ClassDeclaration Class(NamedValueCollection collection) => collection.ToClassModel();
}
