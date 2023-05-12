﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeModels.AbstractCodeModels;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Extensions;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Access;
using CodeModels.Models.Primitives.Expression.CompileTime;
using CodeModels.Models.Primitives.Expression.Instantiation;
using CodeModels.Models.Primitives.Expression.Invocation;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Models.Primitives.Member;
using CodeModels.Reflection;
using Common.Extensions;
using Common.Reflection;
using Generator.Models.Primitives.Expression.AnonymousFunction;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Factory;

public class CodeModelParser
{
    private SemanticModel model;
    private IProgramContext context;

    public CodeModelParser(IProgramContext context, SemanticModel model)
    {
        this.context = context;
        this.model = model;
    }

    public void Register(CompilationUnitSyntax node)
    {
        foreach (var member in node.Members)
        {
            var symbol = model.GetDeclaredSymbol(member);
            if (symbol is INamedTypeSymbol namedTypeSymbol)
            {
                foreach (var constructor in namedTypeSymbol.Constructors)
                {

                }
                foreach (var memberInner in namedTypeSymbol.GetMembers())
                {
                    if (memberInner is IFieldSymbol fieldSymbol)
                    {
                        var field = Parse(fieldSymbol);
                        Register(fieldSymbol, field);
                    }
                }
            }

        }
        //foreach(var member in symbol.mem)
    }
    private T Register2<T>(SyntaxNode node, T model) where T : ICodeModel => context.Register(node, model);
    private T Register<T>(ISymbol symbol, T model) where T : ICodeModel => context.Register(symbol, model);
    private T Register<T>(SyntaxNode node, T codeModel) where T : ICodeModel
        => SymbolUtils.GetDeclaration(node, model) is ISymbol symbol ? Register(symbol, codeModel) : codeModel;
    private T GetModel<T>(ISymbol symbol) where T : class, ICodeModel
        => context.Get<T>(symbol);
    private T? TryGetModel<T>(ISymbol symbol) where T : class, ICodeModel
        => context.TryGet<T>(symbol);
    public IType Parse(SyntaxToken token) => ParseType(token.ToString());
    public IType ParseType(string identifier) => ParseType(ParseTypeName(identifier));
    public IType Parse(Microsoft.CodeAnalysis.TypeInfo typeInfo) => typeInfo.Type is null && typeInfo.ConvertedType is null ? ParseType(typeInfo.ToString()) : Parse(typeInfo.Type ?? typeInfo.ConvertedType);  // TODO: Nullability
    public IType Parse(ITypeSymbol symbol) => SymbolUtils.IsNewDefined(symbol) ? TryGetModel<IClassDeclaration>(symbol)?.Get_Type() ?? new TypeFromSymbol(symbol) : new TypeFromSymbol(symbol);

    public IType ParseType(TypeSyntax? syntax, bool required = true, IType? knownType = null)
    {
        var expression = Parse(syntax, required, knownType);
        return (expression is IType type ? type
            : expression is IdentifierExpression identifier
                ? QuickType(identifier.Name) : expression.Get_Type()) ??
             //? Identifier(identifier.Name, symbol: SymbolUtils.GetSymbol(syntax), model: expression) : expression.Get_Type()) ??
             throw new NotImplementedException($"No type for: '{syntax}'.");
    }
    public IExpression Parse(TypeSyntax? syntax, bool required = true, IType? knownType = null) => syntax switch
    {
        ArrayTypeSyntax type => Parse(type, knownType),
        FunctionPointerTypeSyntax type => Parse(type, knownType),
        NameSyntax type => Parse(type, knownType),
        NullableTypeSyntax type => Parse(type, knownType),
        OmittedTypeArgumentSyntax type => Parse(type),
        PointerTypeSyntax type => Parse(type, knownType),
        PredefinedTypeSyntax type => Parse(type, required, knownType),
        RefTypeSyntax type => Parse(type, knownType),
        TupleTypeSyntax type => Parse(type, knownType),
        null => TypeShorthands.NullType,
        _ => throw new NotImplementedException($"TypeSyntax {syntax} not implemented.")
    };

    public IType Parse(ArrayTypeSyntax syntax, IType? type = null) => Models.QuickType.ArrayType(ParseType(syntax.ElementType));
    public IType Parse(FunctionPointerTypeSyntax syntax, IType? type = null) => throw new NotImplementedException();
    public IExpression Parse(NameSyntax syntax, IType? type = null) => syntax switch
    {
        AliasQualifiedNameSyntax name => Parse(name, type),
        QualifiedNameSyntax name => Parse(name, type),
        SimpleNameSyntax name => Parse(name, type),
        _ => throw new NotImplementedException($"NameSyntax {syntax} not implemented.")
    };
    public IType Parse(AliasQualifiedNameSyntax syntax, IType? type = null) => throw new NotImplementedException();
    public IType Parse(QualifiedNameSyntax syntax, IType? type = null)
        => TypeFromReflection.Create(model is null
            ? System.Type.GetType(syntax.ToString())
            : SemanticReflection.GetType(model.GetTypeInfo(syntax).Type));
    public IExpression Parse(SimpleNameSyntax syntax, IType? type = null) => syntax switch
    {
        GenericNameSyntax name => Parse(name, type),
        IdentifierNameSyntax name => Parse(name, type),
        _ => throw new NotImplementedException($"NameSyntax {syntax} not implemented.")
    };
    public IType Parse(GenericNameSyntax syntax, IType? type = null) => QuickType(syntax.ToString());
    public IExpression Parse(IdentifierNameSyntax syntax, IType? type = null)
    {
        var symbol = model is null ? null : model.GetSymbolInfo(syntax).Symbol;
        return model.GetSymbolInfo(syntax).Symbol switch
        {
            IFieldSymbol field => Parse(syntax, field, type),
            IPropertySymbol property => Parse(syntax, property, type),
            //IMethodSymbol method => new MethodFromSymbol(method).Invoke(syntax.ToString(), type, method),
            ITypeSymbol typeSymbol => Parse(typeSymbol),
            INamespaceSymbol namespaceSymbol => Parse(namespaceSymbol),
            _ when syntax.IsKind(SyntaxKind.IdentifierName) => new IdentifierExpression(syntax.ToString(), type, symbol),
            _ when model.GetTypeInfo(syntax) is Microsoft.CodeAnalysis.TypeInfo typeInfo => Parse(typeInfo),
            _ => QuickType(syntax.Identifier.ToString())
        };
    }

    public IExpression Parse(IdentifierNameSyntax syntax, IFieldSymbol field, IType? type = null)
        => SymbolUtils.IsNewDefined(field)
        ? TryGetModel<Field>(field) is Field fieldModel
            ? Register(syntax, new FieldExpression(fieldModel, This(), Scopes: Array.Empty<ICodeModelExecutionScope>(), Symbol: field))
            //? Register(syntax, new FieldModelExpression(, Scopes: Array.Empty<IProgramModelExecutionScope>(), Symbol: field))
            : Register(syntax, new FieldExpressionFromSymbol(field, This(), Scopes: Array.Empty<ICodeModelExecutionScope>()))
        //? new FieldModelExpression! //new FieldModelExpression() 
        : new PropertyFromField(field).AccessValue(syntax.ToString(), type, field);

    public IExpression Parse(IdentifierNameSyntax syntax, IPropertySymbol property, IType? type = null)
         => SymbolUtils.IsNewDefined(property)
        ? TryGetModel<Property>(property) is Property propertyModel
            ? Register(syntax, new PropertyExpression(propertyModel, This(), Scopes: Array.Empty<ICodeModelExecutionScope>(), Symbol: property))
            : Register(syntax, new PropertyExpressionFromSymbol(property, This(), Scopes: Array.Empty<ICodeModelExecutionScope>()))
        : new PropertyFromReflection(property).AccessValue(syntax.ToString(), type, property);

    public IExpression Parse(INamespaceSymbol namespaceSymbol)
         => new Namespace(namespaceSymbol);

    public IType Parse(NullableTypeSyntax syntax, IType? type = null) => ParseType(syntax.ElementType, false);
    public IType Parse(OmittedTypeArgumentSyntax syntax) => throw new NotImplementedException();
    public IType Parse(PointerTypeSyntax syntax, IType? type = null) => throw new NotImplementedException();
    public IType Parse(PredefinedTypeSyntax syntax, IType? type = null)
        => QuickType(syntax.Keyword.ToString());
    public IType Parse(RefTypeSyntax syntax, IType? type = null) => throw new NotImplementedException();
    public IType Parse(TupleTypeSyntax syntax, IType? type = null) => throw new NotImplementedException();

    public Modifier ParseModifier(SyntaxTokenList syntax) => Modifier.None.SetModifiers(syntax.Select(ParseSingleModifier));
    public Modifier ParseSingleModifier(SyntaxToken syntax) => syntax.Kind() switch
    {
        SyntaxKind.PrivateKeyword => Modifier.Private,
        SyntaxKind.ProtectedKeyword => Modifier.Protected,
        SyntaxKind.InternalKeyword => Modifier.Internal,
        SyntaxKind.PublicKeyword => Modifier.Public,
        SyntaxKind.ReadOnlyKeyword => Modifier.Readonly,
        SyntaxKind.ConstKeyword => Modifier.Const,
        SyntaxKind.AbstractKeyword => Modifier.Abstract,
        SyntaxKind.StaticKeyword => Modifier.Static,
        _ => throw new ArgumentException($"Unhandled token '{syntax}'.")
    };

    public IExpression ParseExpression(ExpressionSyntax? syntax, IType? type = null) => syntax switch
    {
        null => VoidValue,
        AnonymousFunctionExpressionSyntax expression => Parse(expression, type),
        AnonymousObjectCreationExpressionSyntax expression => Parse(expression, type),
        ArrayCreationExpressionSyntax expression => Parse(expression, type),
        AssignmentExpressionSyntax expression => Parse(expression, type),
        AwaitExpressionSyntax expression => Parse(expression, type),
        BaseObjectCreationExpressionSyntax expression => Parse(expression, type),
        BinaryExpressionSyntax expression => Parse(expression, type),
        CastExpressionSyntax expression => Parse(expression, type),
        CheckedExpressionSyntax expression => Parse(expression, type),
        ConditionalAccessExpressionSyntax expression => Parse(expression, type),
        ConditionalExpressionSyntax expression => Parse(expression, type),
        DeclarationExpressionSyntax expression => Parse(expression, type),
        DefaultExpressionSyntax expression => Parse(expression, type),
        ElementAccessExpressionSyntax expression => Parse(expression, type),
        ElementBindingExpressionSyntax expression => Parse(expression, type),
        ImplicitArrayCreationExpressionSyntax expression => Parse(expression, type),
        ImplicitElementAccessSyntax expression => Parse(expression, type),
        ImplicitStackAllocArrayCreationExpressionSyntax expression => Parse(expression, type),
        InitializerExpressionSyntax expression => Parse(expression, type ?? TypeShorthands.NullType),
        InstanceExpressionSyntax expression => Parse(expression, type),
        InterpolatedStringExpressionSyntax expression => Parse(expression, type),
        InvocationExpressionSyntax expression => Parse(expression, type),
        IsPatternExpressionSyntax expression => Parse(expression, type),
        LiteralExpressionSyntax expression => Parse(expression, type),
        MakeRefExpressionSyntax expression => Parse(expression, type),
        MemberAccessExpressionSyntax expression => Parse(expression, type),
        MemberBindingExpressionSyntax expression => Parse(expression, type),
        OmittedArraySizeExpressionSyntax expression => Parse(expression, type),
        ParenthesizedExpressionSyntax expression => Parse(expression, type),
        PostfixUnaryExpressionSyntax expression => Parse(expression, type),
        PrefixUnaryExpressionSyntax expression => Parse(expression, type),
        QueryExpressionSyntax expression => Parse(expression, type),
        RangeExpressionSyntax expression => Parse(expression, type),
        RefExpressionSyntax expression => Parse(expression, type),
        RefTypeExpressionSyntax expression => Parse(expression, type),
        RefValueExpressionSyntax expression => Parse(expression, type),
        SizeOfExpressionSyntax expression => Parse(expression, type),
        StackAllocArrayCreationExpressionSyntax expression => Parse(expression, type),
        SwitchExpressionSyntax expression => Parse(expression, type),
        ThrowExpressionSyntax expression => CodeModelParsing.Parse(expression, type),
        TupleExpressionSyntax expression => AbstractCodeModelParsing.Parse(expression, type),
        TypeOfExpressionSyntax expression => Parse(expression, type),
        TypeSyntax expression => Parse(expression, true, type),
        WithExpressionSyntax expression => Parse(expression, type),
        _ => throw new NotImplementedException()
    };

    public BinaryExpression Parse(WithExpressionSyntax syntax, IType? type = null)
        => BinaryExpression(ParseExpression(syntax.Expression), OperationType.With, Parse(syntax.Initializer, type ?? TypeShorthands.NullType));
    public TypeOfExpression Parse(TypeOfExpressionSyntax syntax, IType? type = null)
        => new(type ?? ParseType(syntax.Type));

    public IExpression Parse(SwitchExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public IExpression Parse(StackAllocArrayCreationExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public SizeOfExpression Parse(SizeOfExpressionSyntax syntax, IType? type = null)
        => new(type ?? ParseType(syntax.Type));

    public UnaryExpression Parse(RefValueExpressionSyntax syntax, IType? type = null)
        => UnaryExpression(ParseExpression(syntax.Expression), OperationType.Ref);

    public UnaryExpression Parse(RefTypeExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public IExpression Parse(RefExpressionSyntax syntax, IType? type = null)
        => UnaryExpression(ParseExpression(syntax.Expression), OperationType.Ref);

    public IExpression Parse(RangeExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public IExpression Parse(QueryExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public UnaryExpression Parse(PrefixUnaryExpressionSyntax syntax, IType? type = null) => syntax.Kind() switch
    {
        SyntaxKind.UnaryPlusExpression => UnaryExpression(ParseExpression(syntax.Operand), OperationType.UnaryAdd),
        SyntaxKind.UnaryMinusExpression => UnaryExpression(ParseExpression(syntax.Operand), OperationType.UnarySubtract),
        SyntaxKind.BitwiseNotExpression => UnaryExpression(ParseExpression(syntax.Operand), OperationType.Complement),
        SyntaxKind.LogicalNotExpression => UnaryExpression(ParseExpression(syntax.Operand), OperationType.Not),
        SyntaxKind.PreIncrementExpression => UnaryExpression(ParseExpression(syntax.Operand), OperationType.UnaryAddBefore),
        SyntaxKind.PreDecrementExpression => UnaryExpression(ParseExpression(syntax.Operand), OperationType.UnarySubtractBefore),
        SyntaxKind.AddressOfExpression => UnaryExpression(ParseExpression(syntax.Operand), OperationType.AddressOf),
        SyntaxKind.PointerIndirectionExpression => UnaryExpression(ParseExpression(syntax.Operand), OperationType.PointerIndirection),
        SyntaxKind.IndexExpression => UnaryExpression(ParseExpression(syntax.Operand), OperationType.Index),
        _ => throw new NotImplementedException()
    };

    public UnaryExpression Parse(PostfixUnaryExpressionSyntax syntax, IType? type = null) => syntax.Kind() switch
    {
        SyntaxKind.PostIncrementExpression => UnaryExpression(ParseExpression(syntax.Operand), OperationType.UnaryAddAfter),
        SyntaxKind.PostDecrementExpression => UnaryExpression(ParseExpression(syntax.Operand), OperationType.UnarySubtractAfter),
        SyntaxKind.SuppressNullableWarningExpression => UnaryExpression(ParseExpression(syntax.Operand), OperationType.SuppressNullableWarning),
        _ => throw new NotImplementedException()
    };

    public UnaryExpression Parse(ParenthesizedExpressionSyntax syntax, IType? type = null)
        => UnaryExpression(ParseExpression(syntax.Expression), OperationType.Parenthesis);
    public IExpression Parse(OmittedArraySizeExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public IExpression Parse(MemberBindingExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public MemberAccessExpression Parse(MemberAccessExpressionSyntax syntax, IType? type = null) => syntax.Kind() switch
    {
        SyntaxKind.SimpleMemberAccessExpression => ParseSimpleMemberAccess(syntax),
        SyntaxKind.PointerMemberAccessExpression => throw new NotImplementedException($"PointerMemberAccessExpression not implemented: {syntax}"),
        _ => throw new NotImplementedException($"MemberAccessExpression not implemented: {syntax}")
    };


    public MemberAccessExpression ParseSimpleMemberAccess(MemberAccessExpressionSyntax syntax)
    {
        IType? typeModel = null;
        try
        {
            var symbol = model.GetSymbolInfo(syntax);
            var operation = model?.GetOperation(syntax);
            if (operation is IFieldReferenceOperation fieldReferenceOperation)
            {
                var fieldSymbol = fieldReferenceOperation.Field;
                typeModel = SymbolUtils.IsNewDefined(fieldSymbol)
                    ? GetModel<Field>(fieldSymbol).Type
                    : TypeFromReflection.Create(SemanticReflection.GetType(fieldSymbol));
            }
            else
            {
                var deserializedType = ReflectionSerialization.DeserializeType(syntax.Expression.ToString());
                if (deserializedType is not null)
                {
                    typeModel = TypeFromReflection.Create(deserializedType);
                }
            }
        }
        catch (Exception)
        {

        }
        var expression = ParseExpression(syntax.Expression);
        var accessSymbol = model.GetSymbolInfo(syntax).Symbol;
        return new(expression, new IdentifierExpression(syntax.Name.ToString(), Symbol: accessSymbol), typeModel);
        //return new(expression, Parse(syntax.Name).ToIdentifierExpression(), typeModel);
        //return new(expression, property ?? Parse(syntax.Name).Identifier(), typeModel);
    }

    public IExpression Parse(MakeRefExpressionSyntax syntax, IType? type = null)
        => throw new NotImplementedException();    // TODO

    public PatternExpression Parse(IsPatternExpressionSyntax syntax, IType? type = null)
        => PatternExpression(Parse(syntax.Pattern), ParseExpression(syntax.Expression));

    public IPattern Parse(PatternSyntax syntax) => syntax switch
    {
        BinaryPatternSyntax pattern => Parse(pattern),
        ConstantPatternSyntax pattern => Parse(pattern),
        DeclarationPatternSyntax pattern => Parse(pattern),
        DiscardPatternSyntax pattern => Parse(pattern),
        ParenthesizedPatternSyntax pattern => Parse(pattern),
        RecursivePatternSyntax pattern => Parse(pattern),
        RelationalPatternSyntax pattern => Parse(pattern),
        TypePatternSyntax pattern => Parse(pattern),
        UnaryPatternSyntax pattern => Parse(pattern),
        VarPatternSyntax pattern => Parse(pattern),
        _ => throw new NotImplementedException($"Pattern not implemented: {syntax}")
    };


    public BinaryPattern Parse(BinaryPatternSyntax syntax) => syntax.Kind() switch
    {
        SyntaxKind.OrPattern => new OrPattern(Parse(syntax.Left), Parse(syntax.Right)),
        SyntaxKind.AndPattern => new AndPattern(Parse(syntax.Left), Parse(syntax.Right)),
        _ => throw new NotImplementedException($"Not implemented BinaryPattern: '{syntax}'.")
    };
    public ConstantPattern Parse(ConstantPatternSyntax syntax)
        => new(ParseExpression(syntax.Expression));
    public DeclarationPattern Parse(DeclarationPatternSyntax syntax)
        => new(ParseType(syntax.Type), Parse(syntax.Designation));
    public DiscardPattern Parse(DiscardPatternSyntax syntax)
        => new();
    public ParenthesizedPattern Parse(ParenthesizedPatternSyntax syntax)
        => new(Parse(syntax.Pattern));
    public RecursivePattern Parse(RecursivePatternSyntax syntax)
        => new(ParseType(syntax.Type));
    public RelationalPattern Parse(RelationalPatternSyntax syntax)
        => new(syntax.OperatorToken, ParseExpression(syntax.Expression));
    public TypePattern Parse(TypePatternSyntax syntax)
        => new(ParseType(syntax.Type));
    public UnaryPattern Parse(UnaryPatternSyntax syntax)
        => new(Parse(syntax.Pattern));
    public VarPattern Parse(VarPatternSyntax syntax)
        => new(Parse(syntax.Designation));
    public IVariableDesignation Parse(VariableDesignationSyntax syntax) => syntax switch
    {
        DiscardDesignationSyntax designation => Parse(designation),
        SingleVariableDesignationSyntax designation => Parse(designation),
        ParenthesizedVariableDesignationSyntax designation => Parse(designation),
        _ => throw new NotImplementedException($"Not implemented VariableDesignation: '{syntax}'.")
    };
    public DiscardDesignation Parse(DiscardDesignationSyntax syntax)
        => new();
    public SingleVariableDesignation Parse(SingleVariableDesignationSyntax syntax)
        => new(syntax.Identifier.ToString());
    public ParenthesizedVariableDesignation Parse(ParenthesizedVariableDesignationSyntax syntax)
        => new(syntax.Variables.Select(x => Parse(x)).ToList());

    public CasePatternSwitchLabel Parse(CasePatternSwitchLabelSyntax syntax)
        => new(Parse(syntax.Pattern), syntax.WhenClause is null ? null : Parse(syntax.WhenClause));

    public CaseSwitchLabel Parse(CaseSwitchLabelSyntax syntax)
        => new(ParseExpression(syntax.Value));

    public DefaultSwitchLabel Parse()
        => new();

    public WhenClause Parse(WhenClauseSyntax syntax)
        => new(ParseExpression(syntax.Condition));

    public IExpression Parse(InvocationExpressionSyntax syntax, IType? type = null)
    {
        if (model is null)
        {
            throw new ArgumentException($"No Semantic model. Check the stack where it may have gone.");
        }
        var symbol = model.GetSymbolInfo(syntax);
        IMethodSymbol? methodSymbol = null;
        if (symbol.Symbol is IMethodSymbol foundMethodSymbol)
        {
            methodSymbol = foundMethodSymbol;
        }
        else
        {
            var arguments = syntax.ArgumentList.Arguments.Select(x => model.GetSymbolInfo(x)).ToArray();
            var argaumentTypes = syntax.ArgumentList.Arguments.Select(x => model.GetTypeInfo(x)).ToArray();
            //methodSymbol = (symbol.CandidateSymbols).FirstOrDefault(x => (x as IParameterSymbol).Parameters) as IMethodSymbol;
            methodSymbol = symbol.CandidateSymbols.FirstOrDefault() as IMethodSymbol;
        }
        if (methodSymbol is null)
        {
            throw new ArgumentException($"No Symbol found for {syntax}. Probably Assembly isn't loaded.");
        }

        var expression = ParseExpression(syntax.Expression);
        var caller = expression is MemberAccessExpression access ? access.Expression : expression;
        var argumentExpressions = AbstractCodeModelParsing.ParseNamedValues(syntax.ArgumentList).ToExpressions();
        var argumentList = methodSymbol.MethodKind is MethodKind.ReducedExtension
            ? new IExpression[] { caller }.Concat(argumentExpressions).ToList()
            : argumentExpressions;

        if (SymbolUtils.IsNewDefined(methodSymbol))
        {
            if (TryGetModel<Method>(methodSymbol) is Method method)
                return Invocation(method, caller, argumentList);
            if (TryGetModel<LocalFunctionStatement>(methodSymbol) is LocalFunctionStatement localFunctionStatement)
                return Invocation(localFunctionStatement.ToMethod(), caller, argumentList);
            if (TryGetModel<LocalFunctionExpression>(methodSymbol) is LocalFunctionExpression localFunctionExpression)
                return Invocation(localFunctionExpression.ToMethod(), caller, argumentList);
            throw new ArgumentException($"Not found: {methodSymbol}");
        }
        var methodInfo = SemanticReflection.GetMethod(methodSymbol);
        if (methodInfo is null)
        {
            throw new ArgumentException($"Couldn't find method for symbol: {methodSymbol}");
        }
        return new InvocationFromReflection(methodInfo, caller, argumentList);
    }

    public IExpression Parse(InterpolatedStringExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public IExpression Parse(InstanceExpressionSyntax syntax, IType? type = null) => syntax switch
    {
        BaseExpressionSyntax expression => Parse(expression, type),
        ThisExpressionSyntax expression => Parse(expression, type),
        _ => throw new NotImplementedException()
    };

    public IExpression Parse(BaseExpressionSyntax syntax, IType? type = null) => syntax.Kind() switch
    {
        SyntaxKind.BaseExpression => new IdentifierExpression(syntax.Token.ToString()), // IDK, TODO
        _ => throw new NotImplementedException()
    };
    public IExpression Parse(ThisExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public InitializerExpression Parse(InitializerExpressionSyntax syntax, IType Type) => syntax.Kind() switch
    {
        SyntaxKind.ObjectInitializerExpression => new(Type, syntax.Kind(), AbstractCodeModelParsing.ParseNamedValues(Type, syntax.Expressions)),
        SyntaxKind.CollectionInitializerExpression => new(Type, syntax.Kind(), AbstractCodeModelParsing.ParseNamedValues(Type, syntax.Expressions)),
        SyntaxKind.ArrayInitializerExpression => Type.TypeName switch
        {
            "Dictionary" or "IDictionary" => new(Type, syntax.Kind(), AbstractCodeModelParsing.ParseNamedValues(Type.GetGenericType(1), syntax.Expressions)),
            _ => new(Type, syntax.Kind(), AbstractCodeModelParsing.ParseNamedValues(Type, syntax.Expressions)),
        },
        SyntaxKind.ComplexElementInitializerExpression => new(Type, syntax.Kind(), AbstractCodeModelParsing.ParseNamedValues(Type, syntax.Expressions)),
        SyntaxKind.WithInitializerExpression => new(Type, syntax.Kind(), AbstractCodeModelParsing.ParseNamedValues(Type, syntax.Expressions)),
        _ => throw new NotImplementedException()
    };

    public IExpression Parse(ImplicitStackAllocArrayCreationExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public ImplicitElementAccessExpression Parse(ImplicitElementAccessSyntax syntax, IType? type = null)
        => new(type ?? (model.GetTypeInfo(syntax).Type is ITypeSymbol symbol ? Type(symbol) : TypeShorthands.VoidType), Parse(syntax.ArgumentList).Select(x => x.ToExpression()).ToList());

    public ExpressionCollection Parse(ImplicitArrayCreationExpressionSyntax syntax, IType? type = null)
        => Parse(syntax.Initializer, type ?? Type(model.GetTypeInfo(syntax))).Expressions.ToValueCollection();

    public IExpression Parse(ElementBindingExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public AnyArgExpression<ExpressionSyntax> Parse(ElementAccessExpressionSyntax syntax, IType? type = null)
        => AnyArgExpression(List(ParseExpression(syntax.Expression)).Concat(Parse(syntax.ArgumentList).Select(x => x.Value)).ToList(), OperationType.Bracket);

    public UnaryExpression Parse(DefaultExpressionSyntax syntax, IType? type = null)
        => UnaryExpression(Parse(syntax.Type), OperationType.Default);

    public IExpression Parse(DeclarationExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();

    public TernaryExpression Parse(ConditionalExpressionSyntax syntax, IType? type = null)
        => TernaryExpression(ParseExpression(syntax.Condition), ParseExpression(syntax.WhenTrue), ParseExpression(syntax.WhenFalse));

    public BinaryExpression Parse(ConditionalAccessExpressionSyntax syntax, IType? type = null)
        => BinaryExpression(ParseExpression(syntax.Expression), OperationType.ConditionalAccess, ParseExpression(syntax.WhenNotNull));

    public IExpression Parse(CheckedExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();

    public UnaryExpression Parse(CastExpressionSyntax syntax, IType? type = null)
        => UnaryExpression(ParseExpression(syntax.Expression), OperationType.Cast);

    public BinaryExpression Parse(BinaryExpressionSyntax syntax, IType? type = null) => syntax.Kind() switch
    {
        SyntaxKind.AddExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.Plus, ParseExpression(syntax.Right)),
        SyntaxKind.SubtractExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.Subtract, ParseExpression(syntax.Right)),
        SyntaxKind.MultiplyExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.Multiply, ParseExpression(syntax.Right)),
        SyntaxKind.DivideExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.Divide, ParseExpression(syntax.Right)),
        SyntaxKind.ModuloExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.Modulo, ParseExpression(syntax.Right)),
        SyntaxKind.LeftShiftExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.LeftShift, ParseExpression(syntax.Right)),
        SyntaxKind.RightShiftExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.RightShift, ParseExpression(syntax.Right)),
        SyntaxKind.LogicalOrExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.LogicalOr, ParseExpression(syntax.Right)),
        SyntaxKind.LogicalAndExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.LogicalAnd, ParseExpression(syntax.Right)),
        SyntaxKind.BitwiseOrExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.BitwiseOr, ParseExpression(syntax.Right)),
        SyntaxKind.BitwiseAndExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.BitwiseAnd, ParseExpression(syntax.Right)),
        SyntaxKind.ExclusiveOrExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.ExclusiveOr, ParseExpression(syntax.Right)),
        SyntaxKind.EqualsExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.Equals, ParseExpression(syntax.Right)),
        SyntaxKind.NotEqualsExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.NotEquals, ParseExpression(syntax.Right)),
        SyntaxKind.LessThanExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.LessThan, ParseExpression(syntax.Right)),
        SyntaxKind.LessThanOrEqualExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.LessThanOrEqual, ParseExpression(syntax.Right)),
        SyntaxKind.GreaterThanExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.GreaterThan, ParseExpression(syntax.Right)),
        SyntaxKind.GreaterThanOrEqualExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.GreaterThanOrEqual, ParseExpression(syntax.Right)),
        SyntaxKind.IsExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.Is, ParseExpression(syntax.Right)),
        SyntaxKind.AsExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.As, ParseExpression(syntax.Right)),
        SyntaxKind.CoalesceExpression => BinaryExpression(ParseExpression(syntax.Left), OperationType.Coalesce, ParseExpression(syntax.Right)),
        _ => throw new NotImplementedException()
    };

    public IExpression Parse(BaseObjectCreationExpressionSyntax syntax, IType? type = null) => syntax switch
    {
        ImplicitObjectCreationExpressionSyntax expression => Parse(expression, type ?? Type(model.GetTypeInfo(syntax))),
        ObjectCreationExpressionSyntax expression => Parse(expression, type ?? (SymbolUtils.GetDeclaration(syntax) is ISymbol symbol && SymbolUtils.IsNewDefined(symbol) ? GetModel<IClassDeclaration>(symbol).Get_Type() : Type(model.GetTypeInfo(syntax)))),
        _ => throw new NotImplementedException()
    };

    public ImplicitObjectCreationExpression Parse(ImplicitObjectCreationExpressionSyntax syntax, IType type)
        => new(type, AbstractCodeModelParsing.ParseNamedValues(syntax.ArgumentList), syntax.Initializer is null ? null : Parse(syntax.Initializer, type));
    public ObjectCreationExpression Parse(ObjectCreationExpressionSyntax syntax, IType type)
    {
        var symbol = model?.GetSymbolInfo(syntax).Symbol;
        var op = model?.GetOperation(syntax);
        var t = model?.GetTypeInfo(syntax);
        return new(type, syntax.ArgumentList is null ? null : AbstractCodeModelParsing.ParseNamedValues(syntax.ArgumentList, GetObjectCreationType(syntax, type)),
            syntax.Initializer is null ? null : Parse(syntax.Initializer, GetObjectCreationType(syntax, type)), model?.GetOperation(syntax));
    }

    public IType GetObjectCreationType(ObjectCreationExpressionSyntax syntax, IType type) => syntax switch
    {
        // (syntax.Type as GenericNameSyntax).TypeArgumentList.Arguments
        _ when syntax.Type is GenericNameSyntax genericName && genericName.Identifier.ToString() == "Dictionary" => ParseType(genericName.TypeArgumentList.Arguments.Last()),
        _ => type
    };

    public AwaitExpression Parse(AwaitExpressionSyntax syntax, IType? type = null) => new(ParseExpression(syntax.Expression));

    public IExpression Parse(AssignmentExpressionSyntax syntax, IType? type = null) => syntax.Kind() switch
    {
        SyntaxKind.SimpleAssignmentExpression => new SimpleAssignmentExpression(ParseExpression(syntax.Left, type), ParseExpression(syntax.Right)),
        SyntaxKind.AddAssignmentExpression => new AddAssignmentExpression(ParseExpression(syntax.Left), ParseExpression(syntax.Right)),
        SyntaxKind.SubtractAssignmentExpression => new SubtractAssignmentExpression(ParseExpression(syntax.Left), ParseExpression(syntax.Right)),
        SyntaxKind.MultiplyAssignmentExpression => new MultiplyAssignmentExpression(ParseExpression(syntax.Left), ParseExpression(syntax.Right)),
        SyntaxKind.DivideAssignmentExpression => new DivideAssignmentExpression(ParseExpression(syntax.Left), ParseExpression(syntax.Right)),
        SyntaxKind.ModuloAssignmentExpression => new ModuloAssignmentExpression(ParseExpression(syntax.Left), ParseExpression(syntax.Right)),
        SyntaxKind.AndAssignmentExpression => new AndAssignmentExpression(ParseExpression(syntax.Left), ParseExpression(syntax.Right)),
        SyntaxKind.ExclusiveOrAssignmentExpression => new ExclusiveOrAssignmentExpression(ParseExpression(syntax.Left), ParseExpression(syntax.Right)),
        SyntaxKind.OrAssignmentExpression => new OrAssignmentExpression(ParseExpression(syntax.Left), ParseExpression(syntax.Right)),
        SyntaxKind.LeftShiftAssignmentExpression => new LeftShiftAssignmentExpression(ParseExpression(syntax.Left), ParseExpression(syntax.Right)),
        SyntaxKind.RightShiftAssignmentExpression => new RightShiftAssignmentExpression(ParseExpression(syntax.Left), ParseExpression(syntax.Right)),
        SyntaxKind.CoalesceAssignmentExpression => new CoalesceAssignmentExpression(ParseExpression(syntax.Left), ParseExpression(syntax.Right)),
        _ => throw new NotImplementedException($"Assignment expression {syntax} not implemented.")
    };

    public ExpressionCollection Parse(ArrayCreationExpressionSyntax syntax, IType? type = null)
        => Parse(syntax.Initializer, type ?? Parse(syntax.Type)).Expressions.ToValueCollection();

    public IExpression Parse(AnonymousObjectCreationExpressionSyntax syntax, IType? type = null)
         => throw new NotImplementedException();    // TODO

    public IAnonymousFunctionExpression Parse(AnonymousFunctionExpressionSyntax syntax, IType? type = null) => syntax switch
    {
        AnonymousMethodExpressionSyntax expression => Parse(expression),
        LambdaExpressionSyntax expression => Parse(expression),
        _ => throw new ArgumentException($"Can't parse {nameof(AnonymousObjectCreationExpressionSyntax)} from '{syntax}'.")
    };

    public AnonymousMethodExpression Parse(AnonymousMethodExpressionSyntax syntax)
         => new(ParseModifier(syntax.Modifiers), syntax.AsyncKeyword != default, syntax.DelegateKeyword != default,
             AbstractCodeModelParsing.ParseProperties(syntax.ParameterList), Parse(model.GetTypeInfo(syntax)),
             syntax.Block is null ? null : Parse(syntax.Block),
             syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody));

    public ILambdaExpression Parse(LambdaExpressionSyntax syntax, IType? type = null) => syntax switch
    {
        SimpleLambdaExpressionSyntax expression => Parse(expression),
        ParenthesizedLambdaExpressionSyntax expression => Parse(expression),
        _ => throw new ArgumentException($"Can't parse {nameof(LambdaExpressionSyntax)} from '{syntax}'.")
    };

    public SimpleLambdaExpression Parse(SimpleLambdaExpressionSyntax syntax)
         => new(ParseModifier(syntax.Modifiers), syntax.AsyncKeyword != default,
             CodeModelParsing.Parse(syntax.Parameter), Parse(model.GetTypeInfo(syntax)),
             syntax.Block is null ? null : Parse(syntax.Block),
             syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody));

    public ParenthesizedLambdaExpression Parse(ParenthesizedLambdaExpressionSyntax syntax)
         => new(ParseModifier(syntax.Modifiers), syntax.AsyncKeyword != default,
             AbstractCodeModelParsing.ParseProperties(syntax.ParameterList), Parse(model.GetTypeInfo(syntax)),
             syntax.Block is null ? null : Parse(syntax.Block),
             syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody));

    public LiteralExpression Parse(LiteralExpressionSyntax syntax, IType? type = null) => syntax.Kind() switch
    {
        SyntaxKind.ArgListExpression => NullValue,
        SyntaxKind.NumericLiteralExpression => new(ParseNumber(syntax)),
        SyntaxKind.StringLiteralExpression => new(syntax.Token.ValueText),
        SyntaxKind.CharacterLiteralExpression => new(syntax.Token.ValueText[0]),
        SyntaxKind.TrueLiteralExpression => new(true),
        SyntaxKind.FalseLiteralExpression => new(false),
        SyntaxKind.NullLiteralExpression => NullValue,
        SyntaxKind.DefaultLiteralExpression => DefaultValue,
        _ => throw new ArgumentException($"Unhandled literal kind '{syntax}'.")
    };

    public object ParseNumber(LiteralExpressionSyntax token) => token.Token.ToString().ToUpper() switch
    {
        string s when s.EndsWith("UL") => ulong.Parse(token.Token.ValueText),
        string s when s.LastOrDefault() is 'U' => uint.Parse(token.Token.ValueText),
        string s when s.LastOrDefault() is 'L' => long.Parse(token.Token.ValueText),
        string s when s.LastOrDefault() is 'F' && !s.Contains("X") => float.Parse(token.Token.ValueText),
        string s when s.LastOrDefault() is 'D' => double.Parse(token.Token.ValueText),
        string s when s.LastOrDefault() is 'M' => decimal.Parse(token.Token.ValueText),
        string s when s.Contains('.') => decimal.Parse(token.Token.ValueText),
        string when token.Parent?.ToString().StartsWith("-") ?? false => decimal.Parse(token.Token.ValueText) switch
        {
            // CASTS ARE NEEDED
            > long.MaxValue => decimal.Parse(token.Token.ValueText),
            > int.MaxValue => long.Parse(token.Token.ValueText),
            _ => (object)int.Parse(token.Token.ValueText),
        },
        _ => decimal.Parse(token.Token.ValueText) switch
        {
            // CASTS ARE NEEDED
            > ulong.MaxValue => decimal.Parse(token.Token.ValueText),
            > long.MaxValue => ulong.Parse(token.Token.ValueText),
            > uint.MaxValue => long.Parse(token.Token.ValueText),
            > int.MaxValue => uint.Parse(token.Token.ValueText),
            _ => (object)int.Parse(token.Token.ValueText),
        },
    };

    public IStatement Parse(StatementSyntax syntax) => syntax switch
    {
        BlockSyntax statement => Parse(statement),
        BreakStatementSyntax statement => Parse(statement),
        CheckedStatementSyntax statement => Parse(statement),
        CommonForEachStatementSyntax statement => Parse(statement),
        ContinueStatementSyntax statement => Parse(statement),
        DoStatementSyntax statement => Parse(statement),
        EmptyStatementSyntax statement => Parse(statement),
        ExpressionStatementSyntax statement => Parse(statement),
        FixedStatementSyntax statement => Parse(statement),
        ForStatementSyntax statement => Parse(statement),
        GotoStatementSyntax statement => Parse(statement),
        IfStatementSyntax statement => Parse(statement),
        LabeledStatementSyntax statement => Parse(statement),
        LocalDeclarationStatementSyntax statement => Parse(statement),
        LocalFunctionStatementSyntax statement => Parse(statement),
        LockStatementSyntax statement => Parse(statement),
        ReturnStatementSyntax statement => Parse(statement),
        SwitchStatementSyntax statement => Parse(statement),
        ThrowStatementSyntax statement => Parse(statement),
        TryStatementSyntax statement => Parse(statement),
        UnsafeStatementSyntax statement => Parse(statement),
        UsingStatementSyntax statement => Parse(statement),
        WhileStatementSyntax statement => Parse(statement),
        _ => throw new ArgumentException($"Can't parse {nameof(IStatement)} from '{syntax}'.")
    };

    public Block Parse(BlockSyntax syntax) => new(List(syntax.Statements.Select(x => Parse(x))));
    public BreakStatement Parse(BreakStatementSyntax _) => new();
    public CheckedStatement Parse(CheckedStatementSyntax syntax) => new(Parse(syntax.Block));
    public ForEachStatement Parse(CommonForEachStatementSyntax syntax) => syntax switch
    {
        ForEachStatementSyntax statement => Parse(statement),
        _ => throw new ArgumentException($"Can't parse {nameof(ForEachStatement)} from '{syntax}'.")
    };
    public ForEachStatement Parse(ForEachStatementSyntax syntax) => new(ParseType(syntax.Type), syntax.Identifier.ToString(), ParseExpression(syntax.Expression), Parse(syntax.Statement));
    public ContinueStatement Parse(ContinueStatementSyntax _) => new();
    public DoStatement Parse(DoStatementSyntax syntax) => new(Parse(syntax.Statement), ParseExpression(syntax.Condition));
    public EmptyStatement Parse(EmptyStatementSyntax _) => new();
    public ExpressionStatement Parse(ExpressionStatementSyntax syntax) => new(ParseExpression(syntax.Expression));
    public IMember Parse(GlobalStatementSyntax syntax)
    {
        var statement = Parse(syntax.Statement);
        return statement is ExpressionStatement ? statement : statement;
    }
    public FixedStatement Parse(FixedStatementSyntax syntax) => new(Parse(syntax.Declaration), Parse(syntax.Statement));
    public VariableDeclarations Parse(VariableDeclarationSyntax syntax) => new(ParseType(syntax.Type), Parse(syntax.Variables));
    public VariableDeclarator Parse(VariableDeclaratorSyntax syntax) => new(syntax.Identifier.ToString(), syntax.Initializer is null ? null : ParseExpression(syntax.Initializer.Value));
    public List<VariableDeclarator> Parse(IEnumerable<VariableDeclaratorSyntax> syntax) => syntax.Select(x => Parse(x)).ToList();
    public List<AbstractProperty> Parse(BracketedArgumentListSyntax syntax) => syntax.Arguments.Select(x => Parse(x)).ToList();
    public AbstractProperty Parse(ArgumentSyntax syntax) => new(TypeShorthands.VoidType, syntax.NameColon?.ToString(), ParseExpression(syntax.Expression));  // TODO: Semantics for type
    public List<AbstractProperty> Parse(IEnumerable<ArgumentSyntax> syntax) => syntax.Select(x => Parse(x)).ToList();
    public AttributeList Parse(AttributeListSyntax syntax) => new(syntax.Target is null ? null : Parse(syntax.Target), syntax.Attributes.Select(Parse).ToList());
    public AttributeTargetSpecifier Parse(AttributeTargetSpecifierSyntax syntax) => new(syntax.Identifier.ToString());
    public Models.Attribute Parse(AttributeSyntax syntax)
        => new(syntax.Name.ToString(), new(syntax.ArgumentList is null ? new List<AttributeArgument>() : syntax.ArgumentList.Arguments.Select(Parse).ToList()));
    public AttributeArgument Parse(AttributeArgumentSyntax syntax) => new(ParseExpression(syntax.Expression), syntax.NameColon?.Name.ToString());
    public ForStatement Parse(ForStatementSyntax syntax)
        => new(syntax.Declaration is null ? new(null) : Parse(syntax.Declaration), syntax.Initializers.Select(x => ParseExpression(x)).ToList(), ParseExpression(syntax.Condition), List(syntax.Incrementors.Select(x => ParseExpression(x))), Parse(syntax.Statement));
    public GotoStatement Parse(GotoStatementSyntax syntax) => new(ParseExpression(syntax.Expression));
    public IfStatement Parse(IfStatementSyntax syntax)
        => new(ParseExpression(syntax.Condition), Parse(syntax.Statement), syntax.Else is null ? null : Parse(syntax.Else));
    public IStatement Parse(ElseClauseSyntax syntax) => Parse(syntax.Statement);
    public LabeledStatement Parse(LabeledStatementSyntax syntax) => new(syntax.Identifier.ToString(), Parse(syntax.Statement));
    public LocalDeclarationStatements Parse(LocalDeclarationStatementSyntax syntax) => new(Parse(syntax.Declaration));
    public LocalFunctionStatement Parse(LocalFunctionStatementSyntax syntax)
        => Register2(syntax, new LocalFunctionStatement(ParseModifier(syntax.Modifiers), ParseType(syntax.ReturnType), syntax.Identifier.ToString(),
            ParseTypes(syntax.TypeParameterList),
            AbstractCodeModelParsing.ParseProperties(syntax.ParameterList), Parse(syntax.ConstraintClauses), syntax.Body is null ? null : Parse(syntax.Body),
            syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody.Expression)));
    public List<TypeParameterConstraintClause> Parse(IEnumerable<TypeParameterConstraintClauseSyntax> syntax) => syntax.Select(x => Parse(x)).ToList();
    public TypeParameterConstraintClause Parse(TypeParameterConstraintClauseSyntax syntax)
        => new(syntax.Name.ToString(), syntax.Constraints.Select(x => Parse(x)).ToList());
    public ITypeParameterConstraint Parse(TypeParameterConstraintSyntax syntax) => syntax switch
    {
        ClassOrStructConstraintSyntax constraint => Parse(constraint),
        ConstructorConstraintSyntax constraint => Parse(constraint),
        DefaultConstraintSyntax constraint => Parse(constraint),
        TypeConstraintSyntax constraint => Parse(constraint),
        _ => throw new ArgumentException($"Can't parse {nameof(ITypeParameterConstraint)} from '{syntax}'.")
    };
    public ClassOrStructConstraint Parse(ClassOrStructConstraintSyntax syntax) => new(syntax.Kind(), syntax.ClassOrStructKeyword);
    public ConstructorConstraint Parse(ConstructorConstraintSyntax _) => new();
    public DefaultConstraint Parse(DefaultConstraintSyntax _) => new();
    public TypeConstraint Parse(TypeConstraintSyntax syntax) => new(ParseType(syntax.Type));
    public TypeCollection ParseTypes(TypeParameterListSyntax? syntax) => syntax is null ? new() : new(syntax.Parameters.Select(x => Parse(x)));
    public IType Parse(TypeParameterSyntax syntax) => QuickType(syntax.Identifier.ToString());    // TODO
    public LockStatement Parse(LockStatementSyntax syntax) => new(ParseExpression(syntax.Expression), Parse(syntax.Statement));
    public ReturnStatement Parse(ReturnStatementSyntax syntax) => new(ParseExpression(syntax.Expression));
    public SwitchStatement Parse(SwitchStatementSyntax syntax) => new(ParseExpression(syntax.Expression), List(syntax.Sections.Select(x => Parse(x))));
    public SwitchSection Parse(SwitchSectionSyntax syntax)
        => new(syntax.Labels.Select(x => Parse(x)).ToList(),
           syntax.Statements.Select(x => Parse(x)).ToList());
    public ISwitchLabel Parse(SwitchLabelSyntax syntax) => syntax switch
    {
        CasePatternSwitchLabelSyntax label => Parse(label),
        CaseSwitchLabelSyntax label => Parse(label),
        DefaultSwitchLabelSyntax _ => Parse(),
        _ => throw new NotImplementedException($"SwitchLabelSyntax {syntax} not implemented.")
    };
    public ThrowStatement Parse(ThrowStatementSyntax syntax) => new(ParseExpression(syntax.Expression));
    public TryStatement Parse(TryStatementSyntax syntax)
        => new(Parse(syntax.Block), List(syntax.Catches.Select(x => Parse(x))), syntax.Finally is null ? null : Parse(syntax.Finally));
    public CatchClause Parse(CatchClauseSyntax syntax)
        => new(syntax.Declaration is null ? TypeShorthands.VoidType : ParseType(syntax.Declaration.Type), syntax.Declaration?.Identifier.ToString(), Parse(syntax.Block));
    public CatchDeclaration Parse(CatchDeclarationSyntax syntax) => new(ParseType(syntax.Type), syntax.Identifier.ToString());
    public FinallyClause Parse(FinallyClauseSyntax syntax) => new(Parse(syntax.Block));
    public UnsafeStatement Parse(UnsafeStatementSyntax syntax) => new(Parse(syntax.Block));
    public UsingStatement Parse(UsingStatementSyntax syntax)
        => new(Parse(syntax.Statement),
            syntax.Declaration is null ? null : Parse(syntax.Declaration),
            syntax.Expression is null ? null : ParseExpression(syntax.Expression));
    public WhileStatement Parse(WhileStatementSyntax syntax) => new(ParseExpression(syntax.Condition), Parse(syntax.Statement));

    public CompilationUnit Parse(CompilationUnitSyntax syntax)
            => new(syntax.Members.Select(x => Parse(x)).ToList(), syntax.Usings.Select(Parse).ToList(), syntax.AttributeLists.Select(x => Parse(x)).ToList());
    public UsingDirective Parse(UsingDirectiveSyntax syntax)
            => new(syntax.Name.ToString(), IsGlobal: syntax.GlobalKeyword.IsKind(SyntaxKind.StaticKeyword), IsStatic: syntax.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword), Alias: syntax.Alias?.ToString());

    public IMember Parse(MemberDeclarationSyntax syntax) => syntax switch
    {
        BaseFieldDeclarationSyntax declaration => Parse(declaration),
        BaseMethodDeclarationSyntax declaration => Parse(declaration),
        BaseNamespaceDeclarationSyntax declaration => Parse(declaration),
        BasePropertyDeclarationSyntax declaration => Parse(declaration),
        BaseTypeDeclarationSyntax declaration => Parse(declaration),
        DelegateDeclarationSyntax declaration => Parse(declaration),
        EnumMemberDeclarationSyntax declaration => Parse(declaration),
        GlobalStatementSyntax declaration => Parse(declaration),
        IncompleteMemberSyntax declaration => Parse(declaration),
        _ => throw new NotImplementedException($"Not implemented MemberDeclaration: '{syntax}'.")
    };
    public IMember Parse(BaseFieldDeclarationSyntax syntax) => syntax switch
    {
        EventFieldDeclarationSyntax declaration => Parse(declaration),
        FieldDeclarationSyntax declaration => Parse(declaration),
        _ => throw new NotImplementedException($"Not implemented BaseFieldDeclaration: '{syntax}'.")
    };
    public IMember Parse(EventFieldDeclarationSyntax syntax) => throw new NotImplementedException();
    public Field Parse(FieldDeclarationSyntax syntax)
        => Register(syntax, new Field(Parse(syntax.Declaration).First().Name,
            ParseType(syntax.Declaration.Type), syntax.AttributeLists.Select(x => Parse(x)).ToList(),
            ParseModifier(syntax.Modifiers), ParseExpression(syntax.Declaration.Variables.First()?.Initializer.Value)));
    public Field Parse(IFieldSymbol symbol)
    {

        var x = 0;
        VariableDeclaratorSyntax? variableDeclaratorSyntax = null;
        //VariableDeclarations? declarations = null;
        foreach (var declaring in symbol.DeclaringSyntaxReferences)
        {
            variableDeclaratorSyntax = declaring.GetSyntax() as VariableDeclaratorSyntax;
            //if (variableDeclaraionSyntax is not null)
            //{
            //    declarations = Parse(variableDeclaraionSyntax);
            //}
        }
        //return null!;
        return Register(symbol, new Field(symbol.Name,
                Parse(symbol.Type),
                //symbol.GetAttributes().Select(x => Parse(x)).ToList(),
                new List<AttributeList>(),  // TODO
                ParseModifier(symbol),
                variableDeclaratorSyntax is null ? null : ParseExpression(variableDeclaratorSyntax.Initializer.Value)));
    }

    public Modifier ParseModifier(ISymbol symbol) => Modifier.None.SetFlags(
        symbol.IsStatic ? Modifier.Static : Modifier.None
        | symbol.DeclaredAccessibility switch
        {
            Accessibility.Private => Modifier.Private,
            Accessibility.ProtectedOrInternal => Modifier.Protected,
            Accessibility.Public => Modifier.Public,
            Accessibility.Friend => Modifier.Public,    // TODO
            _ => Modifier.None
        });

    public IMember Parse(BaseMethodDeclarationSyntax syntax) => syntax switch
    {
        ConstructorDeclarationSyntax declaration => Parse(declaration),
        ConversionOperatorDeclarationSyntax declaration => Parse(declaration),
        DestructorDeclarationSyntax declaration => Parse(declaration),
        MethodDeclarationSyntax declaration => Parse(declaration),
        OperatorDeclarationSyntax declaration => Parse(declaration),
        _ => throw new NotImplementedException($"Not implemented BaseMethodDeclaration: '{syntax}'.")
    };
    public IConstructor Parse(ConstructorDeclarationSyntax syntax)
       => model?.GetSymbolInfo(syntax).Symbol is IObjectCreationOperation objectCreationSymbol
        && SemanticReflection.GetConstructor(objectCreationSymbol) is ConstructorInfo constructorInfo
        ? CodeModelsFromReflection.Constructor(constructorInfo)
        : CodeModelFactory.Constructor(Parse(SymbolUtils.GetType(syntax)), new NamedValueCollection(syntax),
            syntax.Body is null ? null : Parse(syntax.Body), syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody.Expression));
    public IMember Parse(ConversionOperatorDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(DestructorDeclarationSyntax syntax) => throw new NotImplementedException();
    public Method Parse(MethodDeclarationSyntax syntax)
         => Register(syntax,
             new Method(syntax.GetName(), new NamedValueCollection(syntax), ParseType(syntax.ReturnType), syntax.Body is null ? null : Parse(syntax.Body), syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody.Expression)));

    public IMember Parse(OperatorDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(BaseNamespaceDeclarationSyntax syntax) => syntax switch
    {
        FileScopedNamespaceDeclarationSyntax declaration => Parse(declaration),
        NamespaceDeclarationSyntax declaration => Parse(declaration),
        _ => throw new NotImplementedException($"Not implemented BaseNamespaceDeclaration: '{syntax}'.")
    };
    public IMember Parse(FileScopedNamespaceDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(NamespaceDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(BasePropertyDeclarationSyntax syntax) => syntax switch
    {
        EventDeclarationSyntax declaration => Parse(declaration),
        IndexerDeclarationSyntax declaration => Parse(declaration),
        PropertyDeclarationSyntax declaration => Parse(declaration),
        _ => throw new NotImplementedException($"Not implemented BasePropertyDeclaration: '{syntax}'.")
    };
    public IMember Parse(EventDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(IndexerDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(PropertyDeclarationSyntax syntax)
        => Register2(syntax, new Property(syntax.Name(), ParseType(syntax.Type),
             syntax.AttributeLists.Select(x => Parse(x)).ToList(), syntax.ExpressionBody is not null
            ? List( Models.Primitives.Member.Accessor.Create(AccessorType.Get, expressionBody: ParseExpression(syntax.ExpressionBody.Expression), modifier: ParseModifier(syntax.Modifiers)))
            : Parse(syntax.AccessorList!),
            ParseModifier(syntax.Modifiers), syntax.Initializer is null ? null : ParseExpression(syntax.Initializer.Value)));
    public List<Accessor> Parse(AccessorListSyntax syntax)
        => List(syntax.Accessors.Select(x => Parse(x)));
    public Accessor Parse(AccessorDeclarationSyntax syntax)
        =>  Models.Primitives.Member.Accessor.Create(AccessorTypeExtensions.FromSyntax(syntax.Kind()), syntax.Body is null ? null : Parse(syntax.Body),
           syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody?.Expression), modifier: ParseModifier(syntax.Modifiers));
    public IMember Parse(BaseTypeDeclarationSyntax syntax) => syntax switch
    {
        EnumDeclarationSyntax declaration => Parse(declaration),
        TypeDeclarationSyntax declaration => Parse(declaration),
        _ => throw new NotImplementedException($"Not implemented BaseTypeDeclaration: '{syntax}'.")
    };
    public IMember Parse(EnumDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(TypeDeclarationSyntax syntax) => syntax switch
    {
        ClassDeclarationSyntax declaration => Parse(declaration),
        InterfaceDeclarationSyntax declaration => Parse(declaration),
        RecordDeclarationSyntax declaration => Parse(declaration),
        StructDeclarationSyntax declaration => Parse(declaration),
        _ => throw new NotImplementedException($"Not implemented BaseTypeDeclaration: '{syntax}'.")
    };
    public ClassDeclaration Parse(ClassDeclarationSyntax @class, NamespaceDeclarationSyntax? @namespace = null) =>
       Register(@class, Class(@class.Identifier.ValueText,
           @class.GetMembers().Select(x => Parse(x)).ToArray(),
           @namespace: @namespace == default ? default : new(@namespace),
           modifier: ParseModifier(@class.Modifiers)));

    public IMember Parse(InterfaceDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(RecordDeclarationSyntax syntax) => syntax switch
    {
        _ when syntax.IsKind(SyntaxKind.RecordDeclaration) => ParseRecordNonStruct(syntax),
        _ when syntax.IsKind(SyntaxKind.RecordStructDeclaration) => ParseRecordStruct(syntax),
        _ => throw new NotImplementedException($"Not implemented RecordDeclaration: '{syntax}'.")
    };
    public IMember ParseRecordNonStruct(RecordDeclarationSyntax syntax)
        => throw new NotImplementedException($"Not implemented RecordDeclaration: '{syntax}'.");
    public IMember ParseRecordStruct(RecordDeclarationSyntax syntax)
        => throw new NotImplementedException($"Not implemented RecordDeclaration: '{syntax}'.");
    public IMember Parse(StructDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(DelegateDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(EnumMemberDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(IncompleteMemberSyntax syntax) => throw new NotImplementedException();
}
