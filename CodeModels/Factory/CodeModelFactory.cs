using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Xml.Linq;
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

    public static CompilationUnit CompilationUnit(List<IMember> members, List<UsingDirective>? usings = null, List<AttributeList>? attributes = null, List<ExternAliasDirective>? externs = null)
        => new(members, usings ?? new List<UsingDirective>(), attributes ?? new List<AttributeList>(), externs);

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
    public static UnaryExpression Default(IType? type) => UnaryExpression(type, OperationType.Default, type);
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
    public static InvocationExpression Invocation(Method method, IExpression? caller, IEnumerable<IExpression>? arguments = null, IEnumerable<ICodeModelExecutionScope>? scopes = null) => new(method, caller, List(arguments), List(scopes));
    public static ConstructorInvocationExpression ConstructorInvocation(Constructor constructor, IEnumerable<IExpression>? arguments = null) => new(constructor, List(arguments));
    //public static OperationCall OperationCall(Method method, IExpression caller, IEnumerable<IExpression>? arguments = null) => new(method, caller, List(arguments));
    public static MemberAccessExpression MemberAccess(Field field, IExpression caller) => new(caller, Identifier(field.Name, model: field));
    public static MemberAccessExpression MemberAccess(EnumMember field, IExpression caller) => new(caller, Identifier(field.Name, model: field));

    public static Argument Arg(string? name, IExpression expression) => Argument.Create(name, expression);
    public static Argument Arg(IExpression expression) => Arg(null, expression);

    public static AttributeList Attributes(AttributeTargetSpecifier? target = null, IEnumerable<Models.Primitives.Attribute.Attribute>? attributes = null)
        => AttributeList.Create(target, attributes);
    public static AttributeList Attributes(IEnumerable<Models.Primitives.Attribute.Attribute> attributes)
        => AttributeList.Create(null, attributes);
    public static Models.Primitives.Attribute.Attribute Attribute(string name, AttributeArgumentList arguments)
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

    public static Field Field(IType? type, string name, IExpression? value = null, IEnumerable<AttributeList>? attributes = null, Modifier modifier = Modifier.None)
        => Models.Primitives.Member.Field.Create(name, type ?? value?.Get_Type() ?? TypeShorthands.NullType, attributes, modifier, value);
    public static Field Field(string name, IExpression? value, Modifier modifier = Modifier.None)
        => Field(value?.Get_Type(), name, modifier: modifier, value: value);
    public static Field Field<T>(string name, IExpression? value = null, Modifier modifier = Modifier.None)
            => Field(Type<T>(), name, modifier: modifier, value: value);

    public static EnumMember EnumField(string name, IExpression? value = null) => EnumMember.Create(name, value: value);
    public static EnumMember EnumField(string name, int value) => EnumField(name, Literal(value));

    public static Property Property(IType? type, string name, IEnumerable<Accessor>? accessors = null, IExpression? value = null, Modifier modifier = Modifier.None, IEnumerable<AttributeList>? attributes = null)
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
    public static IdentifierExpressionGeneric<T> Identifier<T>(string name, ICodeModel? model = null)
        => IdentifierExpressionGeneric<T>.Create(name, model);
    public static List<IStatement> Statements(params IStatement[] statements) => statements.ToList();

    public static Method MethodFull(string name, NamedValueCollection parameters, IType returnType, IEnumerable<IType>? typeParameters = null, IEnumerable<TypeParameterConstraintClause>? constraintClauses = null, Block? body = null, IExpression? expressionBody = null, Modifier? modifier = null)
        => Models.Primitives.Member.Method.Create(name, parameters, returnType, typeParameters, constraintClauses, body, expressionBody, modifier);
    public static Method Method(string name, NamedValueCollection parameters, IType returnType, Block body, IEnumerable<IType>? typeParameters = null, IEnumerable<TypeParameterConstraintClause>? constraintClauses = null, Modifier modifier = Modifier.Public)
        => MethodFull(name, parameters, returnType, typeParameters, constraintClauses, body, null, modifier);
    public static Method Method(string name, NamedValueCollection parameters, IType returnType, IExpression expressionBody, IEnumerable<IType>? typeParameters = null, IEnumerable<TypeParameterConstraintClause>? constraintClauses = null, Modifier modifier = Modifier.Public)
        => MethodFull(name, parameters, returnType, typeParameters, constraintClauses, null, expressionBody, modifier);
    public static Method Method(string name, NamedValueCollection parameters, IType returnType, List<IStatement> statements, Modifier modifier = Modifier.Public)
        => MethodFull(name, parameters, returnType, body: Block(statements), modifier: modifier);
    public static Method Method(string name, IType returnType, Block body, Modifier modifier = Modifier.Public)
        => MethodFull(name, AbstractCodeModelFactory.NamedValues(), returnType, body: body, modifier: modifier);
    public static Method Method(string name, IType returnType, List<IStatement> statements, Modifier modifier = Modifier.Public)
        => MethodFull(name, AbstractCodeModelFactory.NamedValues(), returnType, body: Block(statements), modifier: modifier);
    public static Method Method(string name, IType returnType, IExpression expressionBody, Modifier modifier = Modifier.Public)
        => MethodFull(name, AbstractCodeModelFactory.NamedValues(), returnType, expressionBody: expressionBody, modifier: modifier);

    public static LocalFunctionStatement LocalFunctionFull(string name, NamedValueCollection parameters, IType returnType, IEnumerable<IType>? genericParameters = null, IEnumerable<TypeParameterConstraintClause>? constraintClauses = null, Block? body = null, IExpression? expressionBody = null, Modifier? modifier = null)
        => LocalFunctionStatement.Create(name, parameters, returnType, genericParameters, constraintClauses, body, expressionBody, modifier);
    public static LocalFunctionStatement LocalFunction(string name, NamedValueCollection parameters, IType returnType, Block body, IEnumerable<IType>? genericParameters = null, IEnumerable<TypeParameterConstraintClause>? constraintClauses = null, Modifier modifier = Modifier.Public)
        => LocalFunctionFull(name, parameters, returnType, genericParameters, constraintClauses, body, null, modifier);
    public static LocalFunctionStatement LocalFunction(string name, NamedValueCollection parameters, IType returnType, IExpression expressionBody, IEnumerable<IType>? genericParameters = null, IEnumerable<TypeParameterConstraintClause>? constraintClauses = null, Modifier modifier = Modifier.Public)
        => LocalFunctionFull(name, parameters, returnType, genericParameters, constraintClauses, null, expressionBody, modifier);
    public static LocalFunctionStatement LocalFunction(string name, NamedValueCollection parameters, IType returnType, List<IStatement> statements, Modifier modifier = Modifier.Public)
        => LocalFunctionFull(name, parameters, returnType, body: Block(statements), modifier: modifier);
    public static LocalFunctionStatement LocalFunction(string name, IType returnType, Block body, Modifier modifier = Modifier.Public)
        => LocalFunctionFull(name, AbstractCodeModelFactory.NamedValues(), returnType, body: body, modifier: modifier);
    public static LocalFunctionStatement LocalFunction(string name, IType returnType, List<IStatement> statements, Modifier modifier = Modifier.Public)
        => LocalFunctionFull(name, AbstractCodeModelFactory.NamedValues(), returnType, body: Block(statements), modifier: modifier);
    public static LocalFunctionStatement LocalFunction(string name, IType returnType, IExpression expressionBody, Modifier modifier = Modifier.Public)
        => LocalFunctionFull(name, AbstractCodeModelFactory.NamedValues(), returnType, expressionBody: expressionBody, modifier: modifier);

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

    public static Constructor ConstructorFull(IBaseTypeDeclaration type, NamedValueCollection parameters, Block? body = null, IExpression? expressionBody = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => Models.Primitives.Member.Constructor.Create(type, parameters, body is null && expressionBody is null ? Block() : body, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(IType type, NamedValueCollection parameters, Block? body = null, IExpression? expressionBody = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => Models.Primitives.Member.Constructor.Create(type, parameters, body is null && expressionBody is null ? Block() : body, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(IBaseTypeDeclaration type, NamedValueCollection parameters, IExpression expressionBody,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(type, parameters, null, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(IBaseTypeDeclaration type, NamedValueCollection parameters, List<IStatement> statements, Modifier modifier = Modifier.Public)
        => ConstructorFull(type, parameters, Block(statements), null, modifier);
    public static Constructor Constructor(string type, NamedValueCollection parameters, Block? body = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(Class(type), parameters, body, null, Modifier, Attributes);
    public static Constructor Constructor(string type, NamedValueCollection parameters, IExpression? expressionBody = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(Class(type), parameters, null, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(IBaseTypeDeclaration type, Block? body = null, IExpression? expressionBody = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(type, AbstractCodeModelFactory.NamedValues(), body, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(IBaseTypeDeclaration type, IExpression expressionBody,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(type, AbstractCodeModelFactory.NamedValues(), null, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(IBaseTypeDeclaration type, List<IStatement> statements, Modifier modifier = Modifier.Public)
        => ConstructorFull(type, AbstractCodeModelFactory.NamedValues(), Block(statements), null, modifier);
    public static Constructor Constructor(string type, Block? body = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(Class(type), AbstractCodeModelFactory.NamedValues(), body, null, Modifier, Attributes);
    public static Constructor Constructor(string type, IExpression expressionBody,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(Class(type), AbstractCodeModelFactory.NamedValues(), null, expressionBody, Modifier, Attributes);
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
        => ConstructorFull(VoidClass, AbstractCodeModelFactory.NamedValues(), null, expressionBody, Modifier, Attributes);
    public static Constructor Constructor(List<IStatement> statements, Modifier modifier = Modifier.Public)
        => ConstructorFull(VoidClass, AbstractCodeModelFactory.NamedValues(), Block(statements), null, modifier);
    public static Constructor Constructor(Block? body = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
        => ConstructorFull(VoidClass, AbstractCodeModelFactory.NamedValues(), body, null, Modifier, Attributes);

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

    public static Block Block(IEnumerable<IStatement> statements) => new(List(statements));
    public static Block Block(params IStatement[] statements) => new(List(statements));
    public static Block Block(IStatement statement, bool condition = true) => !condition || statement is Block ? (statement as Block)! : new(List(statement));
    public static Block Block(IExpression expression) => Block(new[] { Statement(expression) });

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

    public static EmptyStatement Empty() => EmptyStatement.Create();
    public static ThisExpression This() => new(TypeShorthands.VoidType);
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

    public static BinaryExpression BinaryExpression(IExpression lhs, OperationType operation, IExpression rhs, IType? type = null)
        => operation.IsBinaryOperator() ? new(lhs, rhs, type ?? TypeShorthands.NullType, operation) : throw new ArgumentException($"Not a binary operator: '{operation}'");

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
        Modifier? modifier = null) => ClassDeclaration.Create(NamespaceUtils.NamePart(name),
            genericParameters, constraintClauses, baseTypeList, members,
            @namespace is null && NamespaceUtils.IsMemberAccess(name) ? Namespace(NamespaceUtils.PathPart(name)) : @namespace, modifier);
    public static ClassDeclaration Class(string name, params IMember[] membersArray) => Class(name, members: membersArray);

    public static InterfaceDeclaration Interface(string name,
        IEnumerable<IType>? genericParameters = null,
        IEnumerable<TypeParameterConstraintClause>? constraintClauses = null,
        IEnumerable<IBaseType>? baseTypeList = null,
        IEnumerable<IMember>? members = null,
        Namespace? @namespace = null,
        Modifier? modifier = null) => InterfaceDeclaration.Create(NamespaceUtils.NamePart(name),
            genericParameters, constraintClauses, baseTypeList, members, @namespace is null && NamespaceUtils.IsMemberAccess(name) ? Namespace(NamespaceUtils.PathPart(name)) : @namespace, modifier);
    public static InterfaceDeclaration Interface(string name, params IMember[] membersArray) => Interface(name, members: membersArray);

    public static EnumDeclaration Enum(string name,
        IEnumerable<IEnumMember>? members = null,
        Namespace? @namespace = null,
        Modifier? modifier = null) => EnumDeclaration.Create(NamespaceUtils.NamePart(name), members, @namespace is null && NamespaceUtils.IsMemberAccess(name) ? Namespace(NamespaceUtils.PathPart(name)) : @namespace, modifier);
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
    bool isDelegate, NamedValueCollection parameters,
    IType type, Block? body = null, IExpression? expressionBody = null)
        => AnonymousMethodExpression.Create(modifier, isAsync, isDelegate, parameters, type, body, expressionBody);

    public static SimpleLambdaExpression SimpleLambda(Modifier modifier,
    bool isAsync,
    INamedValue parameter,
    IType type,
    Block? body = null,
    IExpression? expressionBody = null)
        => SimpleLambdaExpression.Create(modifier, isAsync, parameter, type, body, expressionBody);

    public static ParenthesizedLambdaExpression ParenthesizedLambda(Modifier modifier,
    bool isAsync,
    NamedValueCollection parameters,
    IType type,
    Block? body = null,
    IExpression? expressionBody = null)
        => ParenthesizedLambdaExpression.Create(modifier, isAsync, parameters, type, body, expressionBody);

}
