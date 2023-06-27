using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.AbstractCodeModels.Member;
using CodeModels.Execution.Scope;
using CodeModels.Models;
using CodeModels.Models.Primitives.Attribute;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Access;
using CodeModels.Models.Primitives.Expression.CompileTime;
using CodeModels.Models.Primitives.Expression.Instantiation;
using CodeModels.Models.Primitives.Expression.Invocation;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Models.Primitives.Member;
using CodeModels.Models.Primitives.Member.Enum;
using CodeModels.Reflection;
using CodeModels.Utils;
using Common.Reflection;
using Common.Util;
using Generator.Models.Primitives.Expression.AnonymousFunction;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Factory;

public static class CodeModelFactory
{
    public static readonly LiteralExpression VoidValue = LiteralExpression.Create(TypeShorthands.VoidType);
    public static readonly LiteralExpression NullValue = LiteralExpression.Create(TypeShorthands.NullType);
    public static readonly LiteralExpression DefaultValue = LiteralExpression.Create(TypeShorthands.NullType);
    public static readonly IClassDeclaration VoidClass = Class("_VoidClass");

    public static List<T> List<T>(IEnumerable<T>? objects) => objects?.ToList() ?? new List<T>();
    public static List<T> List<T>(params T[] objects) => objects?.ToList() ?? new List<T>();

    public static Namespace Namespace(string name) => new(name);
    public static Namespace Namespace(Type type) => CodeModelsFromReflection.Namespace(type);

    public static TypeFromReflection Type(Type type) => CodeModelsFromReflection.Type(type);
    public static TypeFromReflection Type<T>() => Type(typeof(T));
    public static QuickType Type(string name, Type? type = null)
        => QuickType(name, type);
    public static QuickType Type(IType type)
        => QuickType(type.TypeName);
    //public static IType Type(IdentifierExpression identifier, SemanticModel? model = null) => CodeModelParsing2.ParseType(identifier.ToString(), model);
    public static IType Type(string code) => CodeModelTypeParsing.Parse(code);
    public static IType Type(IBaseTypeDeclaration declaration) => QuickType(declaration.Name);
    public static IType Type(SyntaxToken token, SemanticModel? model = null) => CodeModelTypeParsing.Parse(token, model);
    public static IType Type(TypeSyntax? type, bool required = true) => CodeModelTypeParsing.ParseType(type, required);
    public static IType Type(Microsoft.CodeAnalysis.TypeInfo typeInfo) => Type(typeInfo.Type!);
    public static IType Type(ITypeSymbol symbol) => Type(SemanticReflection.GetType(symbol));
    public static QuickType QuickType(string identifier,
        Type? type = null,
        ITypeSymbol? symbol = null) => Models.QuickType.Create(identifier, type, symbol);
    public static QuickType QuickType(string identifier,
        IEnumerable<IType> genericTypes,
        Type? type = null,
        ITypeSymbol? symbol = null) => Models.QuickType.Create(identifier, genericTypes, type, symbol);

    public static CompilationUnit CompilationUnit(List<IMember> members, List<UsingDirective>? usings = null, AttributeListList? attributes = null, List<ExternAliasDirective>? externs = null)
        => new(members, usings ?? new List<UsingDirective>(), attributes ?? AttributesList(), externs);

    public static IBaseTypeDeclaration MetodHolder(Type type) => type switch
    {
        { IsInterface: true } => Interface(type),
        { IsEnum: true } => Enum(type),
        _ when ReflectionUtil.IsStatic(type) => StaticClass(type),
        _ => InstanceClass(type)
    };

    public static StaticClassFromReflection StaticClass(Type type) => CodeModelsFromReflection.StaticClass(type);
    public static InstanceClassFromReflection InstanceClass(Type type) => CodeModelsFromReflection.InstanceClass(type);
    public static InterfaceFromReflection Interface(Type type) => CodeModelsFromReflection.Interface(type);
    public static EnumFromReflection Enum(Type type) => CodeModelsFromReflection.Enum(type);

    public static List<IMethod> Methods(Type type) => CodeModelsFromReflection.Methods(type);

    public static IExpression Literal(object? value) => value is IInstantiatedObject o ? o : value is LiteralExpression l ? l : LiteralExpression.Create(value);
    public static UnaryExpression Default(IType? type) => UnaryExpression(type!, OperationType.Default, type);
    public static IExpression Value(object? value) => value switch
    {
        null => NullValue,
        Array array => Values(array),
        _ => Literal(value),
    };
    public static ExpressionCollection Values(IEnumerable<object?> values) => AbstractCodeModelFactory.Expressions(values.Select(Value));
    public static ExpressionCollection Values(Array values) => AbstractCodeModelFactory.Expressions(CollectionUtil.ModernizeArray(values).Select(Value));
    public static ExpressionCollection Values(params object?[] values) => AbstractCodeModelFactory.Expressions(values.Select(Value));

    public static List<IExpression> Literals(IEnumerable<object> values) => values.Select(Literal).ToList();
    public static InvocationExpression Invocation(Method method, IExpression? caller, IEnumerable<IExpression>? arguments = null, IEnumerable<ICodeModelExecutionScope>? scopes = null)
        => new(method, caller, List(arguments), List(scopes));
    public static ConstructorInvocationExpression ConstructorInvocation(Constructor constructor, IEnumerable<IExpression>? arguments = null) => new(constructor, List(arguments));
    //public static OperationCall OperationCall(Method method, IExpression caller, IEnumerable<IExpression>? arguments = null) => new(method, caller, List(arguments));
    public static MemberAccessExpression MemberAccess(IExpression caller, IdentifierExpression identifier) => new(caller, identifier);
    public static MemberAccessExpression MemberAccess(Field field, IExpression caller) => MemberAccess(caller, Identifier(field.Name, model: field));
    public static MemberAccessExpression MemberAccess(EnumMember field, IExpression caller) => MemberAccess(caller, Identifier(field.Name, model: field));
    public static MemberAccessExpression MemberAccess(string caller, string identifier) => MemberAccess(Identifier(caller), Identifier(identifier));


    public static Parameter Param(string name, IType type, IExpression? value = default) => Parameter.Create(name, type, value);
    public static Parameter Param(IType type, IExpression? value = default) => Param(StringUtil.Uncapitalize(type.Name), type, value);
    public static Parameter Param<T>(string? name = null, IExpression? value = default) => Param(name ?? StringUtil.Uncapitalize(typeof(T).Name), Type<T>(), value);
    public static ParameterList ParamList(IEnumerable<IToParameterConvertible>? parameters = default)
        => ParameterList.Create(parameters);
    public static ParameterList ParamList(IToParameterConvertible parameter)
        => ParamList(new[] { parameter.ToParameter() });

    public static Argument Arg(string? name, IExpression expression) => Argument.Create(name, expression);
    public static Argument Arg(IExpression expression) => Arg(null, expression);
    public static ArgumentList ArgList(IEnumerable<Argument>? arguments = default)
        => ArgumentList.Create(arguments);
    public static ArgumentList ArgList(Argument argument) => ArgList(new[] { argument });
    public static ArgumentList ArgList(IEnumerable<IToArgumentConvertible> arguments)
        => ArgumentList.Create(arguments.Select(x => x.ToArgument()));

    public static AttributeList Attributes(AttributeTargetSpecifier? target = null, IEnumerable<IToAttributeConvertible>? attributes = null)
        => AttributeList.Create(target, attributes);
    public static AttributeList Attributes(IEnumerable<IToAttributeConvertible> attributes)
        => AttributeList.Create(null, attributes);

    public static AttributeListList AttributesList(IEnumerable<IToAttributeListConvertible>? attributes = null)
        => AttributeListList.Create(attributes);
    public static AttributeListList AttributesList<T>(IEnumerable<T>? attributes = null) where T : IToAttributeListConvertible
        => AttributeListList.Create(attributes);

    public static Models.Primitives.Attribute.Attribute Attribute(string name, AttributeArgumentList? arguments = null)
        => Models.Primitives.Attribute.Attribute.Create(name, arguments);
    public static AttributeArgumentList AttributeArgList(IEnumerable<AttributeArgument>? arguments = null)
        => AttributeArgumentList.Create(arguments);
    public static AttributeArgument AttributeArg(IExpression expression, string? name = null)
        => AttributeArgument.Create(expression, name);
    public static AttributeTargetSpecifier AttributeTargetSpecifier(string identifier)
        => Models.Primitives.Attribute.AttributeTargetSpecifier.Create(identifier);
    public static NameEquals NameEquals(string identifier)
        => Models.Primitives.Attribute.NameEquals.Create(identifier);
    public static NameColon NameColon(string name)
        => Models.Primitives.Attribute.NameColon.Create(name);

    public static ElementAccessExpression ElementAccess(IType type, IExpression caller, IEnumerable<IExpression>? arguments = null)
        => ElementAccessExpression.Create(type, caller, arguments);
    public static ImplicitElementAccessExpression ImplicitElementAccess(IType type, IEnumerable<IExpression>? arguments = null)
        => ImplicitElementAccessExpression.Create(type, arguments);

    public static Field Field(IType? type, string name, IExpression? value = null, AttributeListList? attributes = null, Modifier modifier = Modifier.None)
        => Models.Primitives.Member.Field.Create(name, type ?? value?.Get_Type() ?? TypeShorthands.NullType, attributes, modifier, value);
    public static Field Field(string name, IExpression? value, Modifier modifier = Modifier.None)
        => Field(value?.Get_Type(), name, modifier: modifier, value: value);
    public static Field Field<T>(string name, IExpression? value = null, Modifier modifier = Modifier.None)
            => Field(Type<T>(), name, modifier: modifier, value: value);

    public static EnumMember EnumField(string name, IExpression? value = null, bool? isImplicitValue = null) => EnumMember.Create(name, value: value, isImplicitValue: isImplicitValue);
    public static EnumMember EnumField(string name, int value, bool isImplicitValue = true) => EnumField(name, Literal(value), isImplicitValue);

    public static Property Property(IType? type, string name, IEnumerable<Accessor>? accessors = null, IExpression? value = null, Modifier modifier = Modifier.None, AttributeListList? attributes = null)
        => Models.Primitives.Member.Property.Create(name, type ?? value?.Get_Type() ?? TypeShorthands.NullType, accessors ?? new Accessor[] { Accessor(AccessorType.Get), Accessor(AccessorType.Set) }, modifier: modifier, value: value, attributes: attributes);
    public static Property Property(string name, IExpression value, IEnumerable<Accessor>? accessors = null, Modifier modifier = Modifier.None)
        => Property(value?.Get_Type(), name, accessors, modifier: modifier, value: value);
    public static Property Property<T>(string name, IExpression? value = null, Modifier modifier = Modifier.None)
            => Property(Type<T>(), name, modifier: modifier, value: value);

    public static Accessor Accessor(AccessorType type,
    Block? body = null,
    IExpression? expressionBody = null,
    IEnumerable<AttributeList>? attributes = null,
    Modifier modifier = Modifier.None) => Models.Primitives.Member.Accessor.Create(type, body, expressionBody, attributes, modifier);

    public static IdentifierExpression IdentifierExp(string name, IType? type = null, ISymbol? symbol = null, ICodeModel? model = null)
        => IdentifierExpression.Create(name, type, symbol, model);
    public static IdentifierExpressionGeneric<T> Identifier<T>(string? name = null, ICodeModel? model = null)
        => IdentifierExpressionGeneric<T>.Create(name ?? StringUtil.Uncapitalize(typeof(T).Name), model);
    public static IdentifierExpressionGeneric<T> StaticIdentifier<T>(ICodeModel? model = null)
        => IdentifierExpressionGeneric<T>.Create(typeof(T).Name, model);
    public static IdentifierExpression Identifier(Type type)
        => IdentifierExpression.Create(type.Name, Type(type));
    public static IdentifierExpression Identifier(IType type)
        => IdentifierExpression.Create(StringUtil.Uncapitalize(type.Name));
    public static List<IStatement> Statements(params IStatement[] statements) => statements.ToList();

    public static Method Method(string name, IToParameterListConvertible parameters, IType returnType, IEnumerable<IType>? typeParameters = null, IEnumerable<TypeParameterConstraintClause>? constraintClauses = null, IStatementOrExpression? body = null, Modifier? modifier = null, MethodBodyPreference? bodyPreference = default)
        => Models.Primitives.Member.Method.Create(name, parameters, returnType, typeParameters, constraintClauses, body, modifier, bodyPreference);
    public static Method Method(string name, IToParameterListConvertible parameters, IType returnType, IStatementOrExpression body, IEnumerable<IType>? typeParameters = null, IEnumerable<TypeParameterConstraintClause>? constraintClauses = null, Modifier? modifier = null, MethodBodyPreference? bodyPreference = default)
        => Method(name, parameters, returnType, typeParameters, constraintClauses, body, modifier, bodyPreference: bodyPreference);
    public static Method Method(string name, IType returnType, IStatementOrExpression body, Modifier? modifier = null, MethodBodyPreference? bodyPreference = default)
        => Method(name, ParamList(), returnType, null, body: body, modifier: modifier, bodyPreference: bodyPreference);

    public static LocalFunctionStatement LocalFunction(string name, IToParameterListConvertible parameters, IType returnType, IEnumerable<IType>? genericParameters = null, IEnumerable<TypeParameterConstraintClause>? constraintClauses = null, IStatementOrExpression? body = null, Modifier? modifier = null, MethodBodyPreference? bodyPreference = default)
        => LocalFunctionStatement.Create(name, parameters, returnType, genericParameters, constraintClauses, body, modifier, bodyPreference);
    public static LocalFunctionStatement LocalFunction(string name, IType returnType, IStatementOrExpression body, Modifier? modifier = default, MethodBodyPreference? bodyPreference = default)
        => LocalFunction(name, ParamList(), returnType, body: body, modifier: modifier, bodyPreference: bodyPreference);

    public static TypeParameterConstraintClause ConstraintClause(string name, IEnumerable<ITypeParameterConstraint>? constraints = null)
        => TypeParameterConstraintClause.Create(name, constraints);
    public static ClassOrStructConstraint ConstraintClassOrStruct(SyntaxKind kind, SyntaxToken classOrStructKeyword)
        => ClassOrStructConstraint.Create(kind, classOrStructKeyword);
    public static ConstructorConstraint ConstraintConstructor() => ConstructorConstraint.Create();
    public static DefaultConstraint ConstraintDefault() => DefaultConstraint.Create();
    public static TypeConstraint ConstraintType(IType type) => TypeConstraint.Create(type);

    public static ICodeModel Member(MemberInfo member) => member switch
    {
        Type type => TypeFromReflection.Create(type),
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

    public static Constructor ConstructorFull(IBaseTypeDeclaration type, IToParameterListConvertible parameters, Block? body = null, IExpression? expressionBody = null,
    Modifier Modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => Models.Primitives.Member.Constructor.Create(type, parameters, body is null && expressionBody is null ? Block() : body, expressionBody, Modifier, attributes, initializer);
    public static Constructor Constructor(IType type, IToParameterListConvertible parameters, Block? body = null, IExpression? expressionBody = null,
    Modifier Modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => Models.Primitives.Member.Constructor.Create(type, parameters, body is null && expressionBody is null ? Block() : body, expressionBody, Modifier, attributes, initializer);
    public static Constructor Constructor(IBaseTypeDeclaration type, IToParameterListConvertible parameters, IExpression expressionBody,
    Modifier Modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => ConstructorFull(type, parameters, null, expressionBody, Modifier, attributes, initializer);
    public static Constructor Constructor(IBaseTypeDeclaration type, IToParameterListConvertible parameters, IEnumerable<IStatement> statements, Modifier modifier = Modifier.Public, ConstructorInitializer? initializer = null)
        => ConstructorFull(type, parameters, Block(statements), null, modifier, initializer: initializer);
    public static Constructor Constructor(string type, IToParameterListConvertible parameters, Block? body = null,
    Modifier Modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => ConstructorFull(Class(type), parameters, body, null, Modifier, attributes, initializer);
    public static Constructor Constructor(string type, IToParameterListConvertible parameters, IExpression? expressionBody = null,
    Modifier Modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => ConstructorFull(Class(type), parameters, null, expressionBody, Modifier, attributes, initializer);
    public static Constructor Constructor(IBaseTypeDeclaration type, Block? body = null, IExpression? expressionBody = null,
    Modifier Modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => ConstructorFull(type, AbstractCodeModelFactory.NamedValues(), body, expressionBody, Modifier, attributes, initializer);
    public static Constructor Constructor(IBaseTypeDeclaration type, IExpression expressionBody,
    Modifier Modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => ConstructorFull(type, AbstractCodeModelFactory.NamedValues(), null, expressionBody, Modifier, attributes, initializer);
    public static Constructor Constructor(IBaseTypeDeclaration type, List<IStatement> statements, Modifier modifier = Modifier.Public, ConstructorInitializer? initializer = null)
        => ConstructorFull(type, AbstractCodeModelFactory.NamedValues(), Block(statements), null, modifier, initializer: initializer);
    public static Constructor Constructor(string type, Block? body = null,
    Modifier Modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => ConstructorFull(Class(type), AbstractCodeModelFactory.NamedValues(), body, null, Modifier, attributes, initializer);
    public static Constructor Constructor(string type, IExpression expressionBody,
    Modifier Modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => ConstructorFull(Class(type), AbstractCodeModelFactory.NamedValues(), null, expressionBody, Modifier, attributes, initializer);
    public static Constructor Constructor(IToParameterListConvertible parameters, IExpression expressionBody,
    Modifier Modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => ConstructorFull(VoidClass, parameters, null, expressionBody, Modifier, attributes, initializer);
    public static Constructor Constructor(IToParameterListConvertible parameters, List<IStatement> statements, Modifier modifier = Modifier.Public, ConstructorInitializer? initializer = null)
        => ConstructorFull(VoidClass, parameters, Block(statements), null, modifier, initializer: initializer);
    public static Constructor Constructor(IToParameterListConvertible parameters, Block? body = null,
    Modifier Modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => ConstructorFull(VoidClass, parameters, body, null, Modifier, attributes, initializer);
    public static Constructor Constructor(IExpression expressionBody,
    Modifier Modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => ConstructorFull(VoidClass, AbstractCodeModelFactory.NamedValues(), null, expressionBody, Modifier, attributes, initializer);
    public static Constructor Constructor(List<IStatement> statements, Modifier modifier = Modifier.Public, ConstructorInitializer? initializer = null)
        => ConstructorFull(VoidClass, AbstractCodeModelFactory.NamedValues(), Block(statements), null, modifier, initializer: initializer);
    public static Constructor Constructor(Block? body = null,
    Modifier Modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => ConstructorFull(VoidClass, AbstractCodeModelFactory.NamedValues(), body, null, Modifier, attributes, initializer: initializer);

    public static ConstructorInitializer BaseConstructorInitializer(IToArgumentListConvertible? arguments = null)
        => ConstructorInitializer.Create(arguments, true);
    public static ConstructorInitializer ThisConstructorInitializer(IToArgumentListConvertible? arguments = null)
        => ConstructorInitializer.Create(arguments, false);

    public static SimpleBaseType SimpleBase(IType type) => SimpleBaseType.Create(type);
    public static SimpleBaseType SimpleBase<T>() => SimpleBase(Type<T>());
    public static List<SimpleBaseType> SimpleBases<T>() => new() { SimpleBase<T>() };
    public static List<SimpleBaseType> SimpleBases<T1, T2>() => new() { SimpleBase<T1>(), SimpleBase<T2>() };
    public static List<SimpleBaseType> SimpleBases<T1, T2, T3>() => new() { SimpleBase<T1>(), SimpleBase<T2>(), SimpleBase<T3>() };
    public static PrimaryConstructorBaseType PrimaryConstructorBase(IType type, IEnumerable<Argument>? arguments = default)
        => PrimaryConstructorBaseType.Create(type, arguments);

    public static VariableDeclarations VariableDeclarations(IType type, IEnumerable<VariableDeclarator>? value = null)
        => Models.VariableDeclarations.Create(type, value);
    public static VariableDeclarations VariableDeclarations(VariableDeclaration? declaration)
        => Models.VariableDeclarations.Create(declaration);
    public static VariableDeclarations VariableDeclarations(IType type, IEnumerable<(string Name, IExpression? Value)> initializations)
        => Models.VariableDeclarations.Create(type, initializations);
    public static VariableDeclarations VariableDeclarations(IType type, string name, IExpression? value = null)
        => Models.VariableDeclarations.Create(type, name, value);
    public static VariableDeclaration VariableDeclaration(IType type, string name, IExpression? value = null)
        => Models.VariableDeclaration.Create(type, name, value);
    public static VariableDeclarator VariableDeclarator(string name, IExpression? value = null)
        => Models.VariableDeclarator.Create(name, value);

    public static Block Block(IEnumerable<IStatementOrExpression> statements) => Models.Block.Create(statements);
    public static Block Block(params IStatementOrExpression[] statements) => Models.Block.Create(statements);
    public static Block Block(IStatementOrExpression? statement, bool condition = true)
        => !condition || statement is Block ? (statement as Block)!
        : Block(statement is null ? Array.Empty<IStatementOrExpression>() : List(statement));

    public static IfStatement If(IExpression condition, IStatement statement, IStatement? @else = null) => new(condition, statement, @else);
    public static MultiIfStatement MultiIf(IEnumerable<IfStatement> ifs, IStatement? @else = null) => new(List(ifs), @else);

    public static ForStatement For(VariableDeclarations? declarations, IEnumerable<IExpression> initializers, IExpression condition, IEnumerable<IExpression> incrementors, IStatement statement, bool blockify = true)
        => ForStatement.Create(declarations, initializers, condition, incrementors, Block(statement, blockify));
    public static ForStatement For(VariableDeclaration? declaration, IExpression? initializer, IExpression condition, IExpression incrementors, IStatement statement, bool blockify = true)
        => ForStatement.Create(declaration, initializer, condition, incrementors, Block(statement, blockify));
    public static ForStatement For(VariableDeclaration? declaration, IExpression condition, IExpression incrementors, IStatement statement, bool blockify = true)
        => For(declaration, null, condition, incrementors, statement, blockify);
    public static ForStatement For(string variable, IExpression limit, IStatement statement, bool blockify = true)
        => For(variable, limit, Block(statement, blockify));
    public static ForStatement For(string variable, IExpression limit, IStatement statement)
        => For(Declaration(Type("int"), variable, Literal(0)),
            null,
            BinaryExpression(Identifier(variable), OperationType.LessThan, limit),
            UnaryExpression(Identifier(variable), OperationType.UnaryAddAfter),
            statement);

    public static ForEachStatement ForEach(IType? type, string identifier, IExpression expression, IStatement statement, bool blockify = true)
        => ForEachStatement.Create(type, identifier, expression, Block(statement, blockify));
    public static ForEachStatement ForEach(string identifier, IExpression expression, IStatement statement, bool blockify = true)
        => ForEach(null, identifier, expression, Block(statement, blockify));

    public static WhileStatement While(IExpression condition, IStatement statement, bool blockify = true)
        => new(condition, Block(statement, blockify));

    public static DoStatement Do(IStatement statement, IExpression condition, bool blockify = true)
        => new(Block(statement, blockify), condition);

    public static SwitchStatement Switch(IExpression expression, IEnumerable<SwitchSection> sections, IStatement? @default = null)
         => @default is null ? new(expression, List(sections)) : new(expression, List(sections), @default);
    public static SwitchSection Case(IExpression label, IStatement statement) => SwitchSection.Create(label, statement);
    public static SwitchSection Cases(IEnumerable<ISwitchLabel> labels, IEnumerable<IStatement> statements) => SwitchSection.Create(labels, statements);
    public static SwitchSection Cases(IEnumerable<IExpression> labels, IStatement statement) => SwitchSection.Create(labels, statement);
    public static CaseSwitchLabel SwitchLabel(IExpression value) => CaseSwitchLabel.Create(value);
    public static DefaultSwitchLabel DefaultLabel() => DefaultSwitchLabel.Create();
    public static WhenClause When(IExpression condition) => WhenClause.Create(condition);

    public static EmptyStatement Empty() => EmptyStatement.Create();
    public static ThisExpression This(IType? type = null) => new(type ?? TypeShorthands.VoidType);
    public static BaseExpression Base(IType? type = null) => BaseExpression.Create(type ?? TypeShorthands.VoidType);
    public static ReturnStatement Return() => ReturnStatement.Create();
    public static ReturnStatement Return(IExpression? expression) => ReturnStatement.Create(expression);
    public static ReturnStatement Return(object? literal) => Return(Literal(literal));
    public static ContinueStatement Continue() => ContinueStatement.Create();
    public static GotoStatement Goto(IExpression expression) => GotoStatement.Create(expression);
    public static BreakStatement Break() => BreakStatement.Create();
    public static CheckedStatement Checked(Block block) => CheckedStatement.Create(block);
    public static UnsafeStatement Unsafe(Block block) => UnsafeStatement.Create(block);
    public static FixedStatement Fixed(VariableDeclarations variableDeclarations, IStatement statement)
        => FixedStatement.Create(variableDeclarations, statement);
    public static LockStatement Lock(IExpression expression, IStatement statement)
        => LockStatement.Create(expression, statement);
    public static LabeledStatement Labeled(string identifier, IStatement statement)
        => LabeledStatement.Create(identifier, statement);

    public static UsingStatement Using(IStatement statement,
        VariableDeclarations? declarations = null,
        IExpression? expression = null)
        => UsingStatement.Create(statement, declarations, expression);

    public static UsingDirective UsingDir(string name,
        bool? isStatic = default,
        bool? isGlobal = default,
        string? alias = default)
        => UsingDirective.Create(name, isStatic, isGlobal, alias);

    public static LocalDeclarationStatements LocalDeclarations(VariableDeclarations declarations, Modifier? modifiers = null)
        => LocalDeclarationStatements.Create(declarations, modifiers);

    public static VariableDeclaration Declaration(IType type, string name, IExpression value) => new(type, name, value);
    public static LocalDeclarationStatement LocalDeclaration(VariableDeclaration declaration, Modifier modifiers = Modifier.None) => new(declaration, modifiers);
    public static LocalDeclarationStatement LocalDeclaration(IType type, string name, IExpression? value = null, Modifier modifiers = Modifier.None)
        => LocalDeclaration(new(type, name, value), modifiers);

    public static DiscardDesignation Discard() => DiscardDesignation.Create();
    public static SingleVariableDesignation SingleVariable(string identifier)
        => SingleVariableDesignation.Create(identifier);
    public static ParenthesizedVariableDesignation ParenthesizedVariable(IEnumerable<IVariableDesignation>? variables = null)
        => ParenthesizedVariableDesignation.Create(variables);


    public static TryStatement Try(IStatement statement, IEnumerable<CatchClause> catchClauses, FinallyClause? @finally = null)
        => new(statement, List(catchClauses), @finally);
    public static TryStatement Try(IStatement statement, CatchClause catchClause, FinallyClause? @finally = null)
        => new(statement, List(catchClause), @finally);
    public static CatchClause Catch(IType type, string? identifier, IStatement statement, CatchFilterClause? filter = null)
        => CatchClause.Create(type, identifier, statement, filter);
    public static CatchClause Catch(IType type, IStatement statement, CatchFilterClause? filter = null)
        => Catch(type, null, statement, filter);
    public static CatchDeclaration CatchDeclaration(IType type, string? identifier = null)
        => Models.CatchDeclaration.Create(type, identifier);
    public static CatchFilterClause CatchFilter(IExpression expression) => CatchFilterClause.Create(expression);
    public static FinallyClause Finally(IStatement statement) => new(statement);
    public static ThrowStatement Throw(IExpression expression) => new(expression);
    public static ThrowExpression ThrowExpression(IExpression expression) => new(expression);

    public static IdentifierExpression Identifier(string name, IType? type = null, ISymbol? symbol = null, ICodeModel? model = null) => new(name, type, symbol, model);

    public static UnaryExpression UnaryExpression(IExpression input, OperationType operation, IType? type = null)
       => operation.IsUnaryOperator() ? new(input, type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not a unary operator: '{operation}'");

    public static UnaryExpression Not(IExpression input, IType? type = null)
       => UnaryExpression(input, OperationType.Not, type);

    public static BinaryExpression BinaryExpression(IExpression lhs, OperationType operation, IExpression rhs, IType? type = null)
        => operation.IsBinaryOperator() ? new(lhs, rhs, type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not a binary operator: '{operation}'");

    public static BinaryExpression Equal(IExpression left, IExpression right) => BinaryExpression(left, OperationType.Equals, right);
    public static BinaryExpression NotEqual(IExpression left, IExpression right) => BinaryExpression(left, OperationType.NotEquals, right);
    public static BinaryExpression Plus(IExpression left, IExpression right) => BinaryExpression(left, OperationType.Plus, right);
    public static BinaryExpression Minus(IExpression left, IExpression right) => BinaryExpression(left, OperationType.Subtract, right);
    public static BinaryExpression Multiply(IExpression left, IExpression right) => BinaryExpression(left, OperationType.Multiply, right);
    public static BinaryExpression Divide(IExpression left, IExpression right) => BinaryExpression(left, OperationType.Divide, right);
    public static BinaryExpression Modulo(IExpression left, IExpression right) => BinaryExpression(left, OperationType.Modulo, right);
    public static BinaryExpression GreaterThan(IExpression left, IExpression right) => BinaryExpression(left, OperationType.GreaterThan, right);
    public static BinaryExpression GreaterThanOrEqual(IExpression left, IExpression right) => BinaryExpression(left, OperationType.GreaterThanOrEqual, right);
    public static BinaryExpression LessThan(IExpression left, IExpression right) => BinaryExpression(left, OperationType.LessThan, right);
    public static BinaryExpression LessThanOrEqual(IExpression left, IExpression right) => BinaryExpression(left, OperationType.LessThanOrEqual, right);
    public static BinaryExpression Or(IExpression left, IExpression right) => BinaryExpression(left, OperationType.LogicalOr, right);
    public static BinaryExpression And(IExpression left, IExpression right) => BinaryExpression(left, OperationType.LogicalAnd, right);
    public static BinaryExpression BitwiseAnd(IExpression left, IExpression right) => BinaryExpression(left, OperationType.BitwiseAnd, right);
    public static BinaryExpression BitwiseOr(IExpression left, IExpression right) => BinaryExpression(left, OperationType.BitwiseOr, right);
    public static BinaryExpression ExclusiveOr(IExpression left, IExpression right) => BinaryExpression(left, OperationType.ExclusiveOr, right);
    public static BinaryExpression LeftShift(IExpression left, IExpression right) => BinaryExpression(left, OperationType.LeftShift, right);
    public static BinaryExpression RightShift(IExpression left, IExpression right) => BinaryExpression(left, OperationType.RightShift, right);
    public static BinaryExpression Coalesce(IExpression left, IExpression right) => BinaryExpression(left, OperationType.Coalesce, right);

    public static IsPatternExpression IsPatternExpression(IExpression lhs, IPattern pattern, IType? type = null)
        => new(lhs, pattern, type ?? TypeShorthands.NullType);

    public static MemberAccessExpression MemberAccess(IExpression lhs, IdentifierExpression property, IType? type = null)
        => new(lhs, property, type);

    public static TernaryExpression TernaryExpression(IExpression input, IExpression output1, IExpression output2, IType? type = null, OperationType operation = OperationType.Ternary)
        => operation.IsTernaryOperator() ? new(input, output1, output2, type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not a ternary operator: '{operation}'");

    public static AnyArgExpression<ExpressionSyntax> AnyArgExpression(IEnumerable<IExpression>? inputs, OperationType operation, IType? type = null)
        => operation.IsAnyArgOperator() ? new(List(inputs), type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not an any arg operator: '{operation}'");

    public static AssignmentExpression Assignment(IExpression left, AssignmentType type, IExpression right) => new(left, right, type);
    public static AssignmentExpression Assignment(IExpression left, IExpression right) => Assignment(left, AssignmentType.Simple, right);
    public static AssignmentExpression AddAssignment(IExpression left, IExpression right) => Assignment(left, AssignmentType.Add, right);
    public static AssignmentExpression SubtractAssignment(IExpression left, IExpression right) => Assignment(left, AssignmentType.Subtract, right);
    public static AssignmentExpression MultiplyAssignment(IExpression left, IExpression right) => Assignment(left, AssignmentType.Multiply, right);
    public static AssignmentExpression DivideAssignment(IExpression left, IExpression right) => Assignment(left, AssignmentType.Divide, right);
    public static AssignmentExpression ModuloAssignment(IExpression left, IExpression right) => Assignment(left, AssignmentType.Modulo, right);
    public static AssignmentExpression AndAssignment(IExpression left, IExpression right) => Assignment(left, AssignmentType.And, right);
    public static AssignmentExpression ExclusiveOrAssignment(IExpression left, IExpression right) => Assignment(left, AssignmentType.ExclusiveOr, right);
    public static AssignmentExpression OrAssignment(IExpression left, IExpression right) => Assignment(left, AssignmentType.Or, right);
    public static AssignmentExpression LeftShiftAssignment(IExpression left, IExpression right) => Assignment(left, AssignmentType.LeftShift, right);
    public static AssignmentExpression RightShiftAssignment(IExpression left, IExpression right) => Assignment(left, AssignmentType.RightShift, right);
    public static AssignmentExpression CoalesceAssignment(IExpression left, IExpression right) => Assignment(left, AssignmentType.Coalesce, right);

    public static ExpressionStatement Statement(IExpression expression) => new(expression);

    public static ClassDeclaration Class(string name,
        IEnumerable<IType>? genericParameters = null,
        IEnumerable<TypeParameterConstraintClause>? constraintClauses = null,
        IEnumerable<IBaseType>? baseTypeList = null,
        IEnumerable<IMember>? members = null,
        Namespace? @namespace = null,
        Modifier? modifier = null,
        AttributeListList? attributes = null) => ClassDeclaration.Create(NamespaceUtils.NamePart(name),
            genericParameters, constraintClauses, baseTypeList, members,
            @namespace is null && NamespaceUtils.IsMemberAccess(name) ? Namespace(NamespaceUtils.PathPart(name)) : @namespace, modifier, attributes);
    public static ClassDeclaration Class(string name, params IMember[] membersArray) => Class(name, members: membersArray);

    public static InterfaceDeclaration Interface(string name,
        IEnumerable<IType>? genericParameters = null,
        IEnumerable<TypeParameterConstraintClause>? constraintClauses = null,
        IEnumerable<IBaseType>? baseTypeList = null,
        IEnumerable<IMember>? members = null,
        Namespace? @namespace = null,
        Modifier? modifier = null,
        AttributeListList? attributes = null) => InterfaceDeclaration.Create(NamespaceUtils.NamePart(name),
            genericParameters, constraintClauses, baseTypeList, members,
            @namespace is null && NamespaceUtils.IsMemberAccess(name) ? Namespace(NamespaceUtils.PathPart(name)) : @namespace,
            modifier, attributes);
    public static InterfaceDeclaration Interface(string name, params IMember[] membersArray) => Interface(name, members: membersArray);

    public static EnumDeclaration Enum(string name,
        IEnumerable<IEnumMember>? members = null,
        Namespace? @namespace = null,
        Modifier? modifier = null,
        AttributeListList? attributes = null) => EnumDeclaration.Create(NamespaceUtils.NamePart(name), members,
            @namespace is null && NamespaceUtils.IsMemberAccess(name) ? Namespace(NamespaceUtils.PathPart(name)) : @namespace,
            modifier, attributes);
    public static EnumDeclaration Enum(string name, params IEnumMember[] membersArray) => Enum(name, members: membersArray);
    public static EnumDeclaration Enum(string name, params string[] memberNames) => Enum(name, members: memberNames.Select(x => EnumField(x)).ToArray());

    public static TryStatement TryStatement(IStatement statement, IEnumerable<CatchClause>? catchClauses, FinallyClause? @finally = null)
        => Models.TryStatement.Create(statement, catchClauses, @finally);

    public static ObjectCreationExpression ObjectCreation(IType type,
        IEnumerable<IExpression>? arguments = null,
        InitializerExpression? initializer = null,
        Microsoft.CodeAnalysis.IOperation? operation = null)
        => ObjectCreationExpression.Create(type,
            arguments,
            initializer,
            operation);

    public static ImplicitObjectCreationExpression ImplicitObjectCreation(IType type,
        IEnumerable<IExpression>? arguments = null,
        InitializerExpression? initializer = null,
        Microsoft.CodeAnalysis.IOperation? operation = null)
        => ImplicitObjectCreationExpression.Create(type,
            arguments,
            initializer,
            operation);

    public static ArrayCreationExpression ArrayCreation(IType type,
        IEnumerable<List<IExpression>>? arguments = null,
        InitializerExpression? initializer = null,
        Microsoft.CodeAnalysis.IOperation? operation = null)
        => ArrayCreationExpression.Create(type,
            arguments,
            initializer,
            operation);

    public static InitializerExpression ArrayInitializer(IEnumerable<IExpression> expressions, IType? type = null)
        => Initializer(expressions, SyntaxKind.ArrayInitializerExpression, type);
    public static InitializerExpression ObjectInitializer(IEnumerable<IExpression> expressions, IType? type = null)
        => Initializer(expressions, SyntaxKind.ObjectInitializerExpression, type);
    public static InitializerExpression CollectionInitializer(IEnumerable<IExpression> expressions, IType? type = null)
        => Initializer(expressions, SyntaxKind.CollectionInitializerExpression, type);
    public static InitializerExpression ComplexElementInitializer(IEnumerable<IExpression> expressions, IType? type = null)
        => Initializer(expressions, SyntaxKind.ComplexElementInitializerExpression, type);
    public static InitializerExpression WithInitializer(IEnumerable<IExpression> expressions, IType? type = null)
        => Initializer(expressions, SyntaxKind.WithInitializerExpression, type);
    public static InitializerExpression Initializer(IEnumerable<IExpression> expressions, SyntaxKind kind, IType? type = null)
        => new(type ?? TypeUtil.FindCommonType(expressions.Select(x => x.Get_Type())), kind, List(expressions));

    public static AwaitExpression Await(IExpression expression) => AwaitExpression.Create(expression);
    public static TypeOfExpression TypeOf(IType type) => TypeOfExpression.Create(type);
    public static SizeOfExpression SizeOf(IType type) => SizeOfExpression.Create(type);

    public static AnonymousMethodExpression AnonymousMethod(Modifier modifier, bool isAsync,
    bool isDelegate, IToParameterListConvertible parameters,
    IType type, Block? body = null, IExpression? expressionBody = null)
        => AnonymousMethodExpression.Create(modifier, isAsync, isDelegate, parameters, type, body, expressionBody);

    public static ILambdaExpression Lambda(IToParameterListConvertible parameters,
    IType type,
    IStatementOrExpression? body = null,
    bool? isAsync = default,
    Modifier? modifier = default,
    MethodBodyPreference? bodyPreference = default)
        => parameters.ToParameterList() is ParameterList { Parameters.Count: 1 } p
        ? SimpleLambda(p.Parameters.First(), type, body, isAsync, modifier, bodyPreference)
        : ParenthesizedLambda(parameters, type, body, isAsync, modifier, bodyPreference);

    public static SimpleLambdaExpression SimpleLambda(INamedValue parameter,
    IType type,
    IStatementOrExpression? body = null,
    bool? isAsync = default,
    Modifier? modifier = default,
    MethodBodyPreference? bodyPreference = default)
        => SimpleLambdaExpression.Create(parameter, type, body, isAsync, bodyPreference, modifier);

    public static ParenthesizedLambdaExpression ParenthesizedLambda(IToParameterListConvertible parameters,
    IType type,
    IStatementOrExpression? body = null,
    bool? isAsync = default,
    Modifier? modifier = default,
    MethodBodyPreference? bodyPreference = default)
        => ParenthesizedLambdaExpression.Create(parameters, type, body, isAsync, bodyPreference, modifier);

    public static ConstantPattern ConstantPat(IExpression expression) => ConstantPattern.Create(expression);
    public static DeclarationPattern DeclarationPat(IType type, IVariableDesignation designation)
        => DeclarationPattern.Create(type, designation);
    public static DiscardPattern DiscardPat() => DiscardPattern.Create();
    public static ParenthesizedPattern ParenthesizedPat(IPattern pattern) => ParenthesizedPattern.Create(pattern);
    public static RecursivePattern RecursivePat(IType type) => RecursivePattern.Create(type);
    public static RelationalPattern RelationalPat(SyntaxToken @operator, IExpression expression)
        => RelationalPattern.Create(@operator, expression);
    public static TypePattern TypePat(IType type) => TypePattern.Create(type);
    public static UnaryPattern UnaryPat(IPattern pattern) => UnaryPattern.Create(pattern);
    public static VarPattern VarPat(IVariableDesignation designation) => VarPattern.Create(designation);
    public static ListPattern ListPat(IEnumerable<IPattern>? patterns = null, IVariableDesignation? designation = null)
        => ListPattern.Create(patterns, designation);
    public static SlicePattern SlicePat(IPattern? pattern) => SlicePattern.Create(pattern);
    public static CasePatternSwitchLabel CasePatSwitchLabel(IPattern pattern, WhenClause? whenClause = default)
        => CasePatternSwitchLabel.Create(pattern, whenClause);

    public static T T<T>() => default!;
}
