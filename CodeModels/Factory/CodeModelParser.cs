using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.AbstractCodeModels.Member;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Extensions;
using CodeModels.Models;
using CodeModels.Models.Primitives.Attribute;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Access;
using CodeModels.Models.Primitives.Expression.CompileTime;
using CodeModels.Models.Primitives.Expression.Instantiation;
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
using static CodeModels.Parsing.ParseUtil;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Factory;

public class CodeModelParser
{
    private SemanticModel model { get; init; }
    private IProgramContext Context { get; init; }
    public CompilationUnitSyntax CompilationUnit { get; init; }

    public CodeModelParser(SemanticModel model, CompilationUnitSyntax compilationUnit, IProgramContext context)
    {
        this.model = model;
        this.CompilationUnit = compilationUnit;
        this.Context = context;
    }

    public CodeModelParser(string code)
    {
        var model = code.ParseAndKeepSemanticModel();
        this.model = model.Model;
        this.CompilationUnit = model.Compilation;
        this.Context = ProgramContext.NewContext(model.Compilation, model.Model);
    }

    public CodeModelParser(SemanticModel model, CompilationUnitSyntax compilationUnit)
        : this(model, compilationUnit, ProgramContext.NewContext(compilationUnit, model))
    { }

    private T Register<T>(ISymbol symbol, T model) where T : ICodeModel
        => Context.Register(symbol, model);
    private T Register<T>(SyntaxNode node, T codeModel) where T : ICodeModel
        => Context.Register(node, codeModel);
    private T GetModel<T>(ISymbol symbol) where T : class, ICodeModel
        => Context.Get<T>(symbol);
    private T? TryGetModel<T>(ISymbol symbol) where T : class, ICodeModel
        => Context.TryGet<T>(symbol);
    public IType Parse(SyntaxToken token) => ParseType(token.ToString());
    public IType ParseType(string identifier) => ParseType(ParseTypeName(identifier));
    public IType Parse(Microsoft.CodeAnalysis.TypeInfo typeInfo) => typeInfo.Type is null && typeInfo.ConvertedType is null
        ? ParseType(typeInfo.ToString())
        : Parse(typeInfo.Type ?? typeInfo.ConvertedType ?? throw new NotImplementedException());
    public IType Parse(ITypeSymbol symbol) => SymbolUtils.IsNewDefined(symbol)
        ? TryGetModel<IClassDeclaration>(symbol)?.Get_Type() ?? TypeFromSymbol.Create(symbol)
        : TypeFromSymbol.Create(symbol, SemanticReflection.GetType(symbol));

    public IType ParseType(TypeSyntax? syntax, bool required = true, IType? knownType = null)
    {
        var expression = Parse(syntax, knownType);
        return (expression is IType type ? (required ? type : type.ToOptionalType())
            : expression is IdentifierExpression identifier
                ? QuickType(required ? identifier.Name : $"{identifier.Name}?", type: ReflectionSerialization.IsShortHandName(identifier.Name)
                    ? ReflectionSerialization.DeserializeTypeLookAtShortNames(identifier.Name, !required)
                    : null)
                : expression.Get_Type()) ??
             //? Identifier(identifier.Name, symbol: SymbolUtils.GetSymbol(syntax): expression) : expression.Get_Type()) ??
             throw new NotImplementedException($"No type for: '{syntax}'.");
    }
    public IExpression Parse(TypeSyntax? syntax, IType? knownType = null) => syntax switch
    {
        ArrayTypeSyntax type => Parse(type, knownType),
        FunctionPointerTypeSyntax type => Parse(type, knownType),
        NameSyntax type => Parse(type, knownType),
        NullableTypeSyntax type => Parse(type, knownType),
        OmittedTypeArgumentSyntax type => Parse(type),
        PointerTypeSyntax type => Parse(type, knownType),
        PredefinedTypeSyntax type => Parse(type, knownType),
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
            : SemanticReflection.GetType(model.GetTypeInfo(syntax).Type ?? throw new NotImplementedException()));
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
        ? TryGetModel<IField>(field) is IField fieldModel
            ? new FieldExpression(fieldModel, This(), Scopes: Array.Empty<ICodeModelExecutionScope>(), Symbol: field)
            : new FieldExpressionFromSymbol(field, This(), Scopes: Array.Empty<ICodeModelExecutionScope>())
        : new FieldFromReflection(field).AccessValue(syntax.ToString(), type, field);

    public IExpression Parse(IdentifierNameSyntax syntax, IPropertySymbol property, IType? type = null)
        => SymbolUtils.IsNewDefined(property)
            ? TryGetModel<IProperty>(property) is IProperty propertyModel
                ? new PropertyExpression(propertyModel, This(), Scopes: Array.Empty<ICodeModelExecutionScope>(), Symbol: property)
                : new PropertyExpressionFromSymbol(property, This(), Scopes: Array.Empty<ICodeModelExecutionScope>())
            : new PropertyFromReflection(property).AccessValue(syntax.ToString(), type, property);

    public IExpression Parse(INamespaceSymbol namespaceSymbol)
         => new Namespace(namespaceSymbol);

    public IType Parse(NullableTypeSyntax syntax, IType? type = null) => ParseType(syntax.ElementType, false);
    public IType Parse(OmittedTypeArgumentSyntax syntax) => throw new NotImplementedException();
    public IType Parse(PointerTypeSyntax syntax, IType? type = null) => throw new NotImplementedException();
    public IType Parse(PredefinedTypeSyntax syntax, IType? type = null)
        => QuickType(syntax.Keyword.ToString(), type: ReflectionSerialization.DeserializeTypeLookAtShortNames(syntax.Keyword.ToString()));
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
        SyntaxKind.StaticKeyword => Modifier.Static,
        SyntaxKind.AbstractKeyword => Modifier.Abstract,
        SyntaxKind.VirtualKeyword => Modifier.Virtual,
        SyntaxKind.OverrideKeyword => Modifier.Override,
        SyntaxKind.NewKeyword => Modifier.New,
        SyntaxKind.FileKeyword => Modifier.File,
        _ => throw new ArgumentException($"Unhandled token '{syntax}'.")
    };

    public Modifier Modifiers(SyntaxTokenList tokenList) => ParseModifier(tokenList);

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
        OmittedArraySizeExpressionSyntax expression => Parse(expression),
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
        ThrowExpressionSyntax expression => Parse(expression, type),
        TupleExpressionSyntax expression => AbstractCodeModelParsing.Parse(null!, expression, type),
        TypeOfExpressionSyntax expression => Parse(expression, type),
        TypeSyntax expression => Parse(expression, type),
        WithExpressionSyntax expression => Parse(expression, type),
        _ => throw new NotImplementedException()
    };

    public BinaryExpression Parse(WithExpressionSyntax syntax, IType? type = null)
        => BinaryExpression(ParseExpression(syntax.Expression), OperationType.With, Parse(syntax.Initializer, type ?? TypeShorthands.NullType));

    public TypeOfExpression Parse(TypeOfExpressionSyntax syntax, IType? type = null)
        => TypeOf(type ?? ParseType(syntax.Type));

    public ThrowExpression Parse(ThrowExpressionSyntax syntax, IType? type = null)
        => ThrowExpression(ParseExpression(syntax.Expression));

    public IExpression Parse(SwitchExpressionSyntax syntax, IType? type = null)
        => throw new NotImplementedException();    // TODO

    public IExpression Parse(StackAllocArrayCreationExpressionSyntax syntax, IType? type = null)
        => throw new NotImplementedException();    // TODO

    public SizeOfExpression Parse(SizeOfExpressionSyntax syntax, IType? type = null)
        => SizeOf(type ?? ParseType(syntax.Type));

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
    public IExpression Parse(OmittedArraySizeExpressionSyntax _) => VoidValue;

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
            var operation = model.GetOperation(syntax);
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
        return MemberAccess(expression, IdentifierExp(syntax.Name.ToString(), symbol: accessSymbol), typeModel);
    }

    public IExpression Parse(MakeRefExpressionSyntax syntax, IType? type = null)
        => throw new NotImplementedException();    // TODO

    public IsPatternExpression Parse(IsPatternExpressionSyntax syntax, IType? type = null)
        => IsPatternExpression(ParseExpression(syntax.Expression), Parse(syntax.Pattern));

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
        ListPatternSyntax pattern => Parse(pattern),
        SlicePatternSyntax pattern => Parse(pattern),
        _ => throw new NotImplementedException($"Pattern not implemented: {syntax}")
    };

    public BinaryPattern Parse(BinaryPatternSyntax syntax) => syntax.Kind() switch
    {
        SyntaxKind.OrPattern => new OrPattern(Parse(syntax.Left), Parse(syntax.Right)),
        SyntaxKind.AndPattern => new AndPattern(Parse(syntax.Left), Parse(syntax.Right)),
        _ => throw new NotImplementedException($"Not implemented BinaryPattern: '{syntax}'.")
    };
    public ConstantPattern Parse(ConstantPatternSyntax syntax)
        => ConstantPat(ParseExpression(syntax.Expression));
    public DeclarationPattern Parse(DeclarationPatternSyntax syntax)
        => DeclarationPat(ParseType(syntax.Type), Parse(syntax.Designation));
    public DiscardPattern Parse(DiscardPatternSyntax _)
        => DiscardPat();
    public ParenthesizedPattern Parse(ParenthesizedPatternSyntax syntax)
        => ParenthesizedPat(Parse(syntax.Pattern));
    public RecursivePattern Parse(RecursivePatternSyntax syntax)
        => RecursivePat(ParseType(syntax.Type));
    public RelationalPattern Parse(RelationalPatternSyntax syntax)
        => RelationalPat(syntax.OperatorToken, ParseExpression(syntax.Expression));
    public TypePattern Parse(TypePatternSyntax syntax)
        => TypePat(ParseType(syntax.Type));
    public UnaryPattern Parse(UnaryPatternSyntax syntax)
        => UnaryPat(Parse(syntax.Pattern));
    public VarPattern Parse(VarPatternSyntax syntax)
        => VarPat(Parse(syntax.Designation));
    public ListPattern Parse(ListPatternSyntax syntax)
        => ListPat(syntax.Patterns.Select(Parse).ToArray(), syntax.Designation is null ? null : Parse(syntax.Designation));
    public SlicePattern Parse(SlicePatternSyntax syntax)
        => SlicePat(syntax.Pattern is null ? null : Parse(syntax.Pattern));

    public IVariableDesignation Parse(VariableDesignationSyntax syntax) => syntax switch
    {
        DiscardDesignationSyntax designation => Parse(designation),
        SingleVariableDesignationSyntax designation => Parse(designation),
        ParenthesizedVariableDesignationSyntax designation => Parse(designation),
        _ => throw new NotImplementedException($"Not implemented VariableDesignation: '{syntax}'.")
    };
    public DiscardDesignation Parse(DiscardDesignationSyntax _)
        => Discard();
    public SingleVariableDesignation Parse(SingleVariableDesignationSyntax syntax)
        => SingleVariable(syntax.Identifier.ToString());
    public ParenthesizedVariableDesignation Parse(ParenthesizedVariableDesignationSyntax syntax)
        => ParenthesizedVariable(syntax.Variables.Select(Parse).ToList());

    public CasePatternSwitchLabel Parse(CasePatternSwitchLabelSyntax syntax)
        => CasePatSwitchLabel(Parse(syntax.Pattern), syntax.WhenClause is null ? null : Parse(syntax.WhenClause));

    public CaseSwitchLabel Parse(CaseSwitchLabelSyntax syntax)
        => SwitchLabel(ParseExpression(syntax.Value));

    public DefaultSwitchLabel ParseDefaultSwitch()
        => DefaultLabel();

    public WhenClause Parse(WhenClauseSyntax syntax)
        => When(ParseExpression(syntax.Condition));

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
        var argumentExpressions = AbstractCodeModelParsing.ParseNamedValues(this, syntax.ArgumentList).ToExpressions();
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
        return CodeModelsFromReflection.Invocation(methodInfo, caller, argumentList);
    }

    public IExpression Parse(InterpolatedStringExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public IExpression Parse(InstanceExpressionSyntax syntax, IType? type = null) => syntax switch
    {
        BaseExpressionSyntax expression => Parse(expression, type),
        ThisExpressionSyntax expression => Parse(expression, type),
        _ => throw new NotImplementedException()
    };

    public IExpression Parse(BaseExpressionSyntax _, IType? type = null) => Base(type);
    public IExpression Parse(ThisExpressionSyntax _, IType? type = null) => This(type);

    public InitializerExpression Parse(InitializerExpressionSyntax syntax, IType type) =>
        Initializer(syntax.Expressions.Select(x => ParseExpression(x, type)), syntax.Kind(), type);

    public IExpression Parse(ImplicitStackAllocArrayCreationExpressionSyntax syntax, IType? type = null)
        => throw new NotImplementedException();    // TODO

    public ImplicitElementAccessExpression Parse(ImplicitElementAccessSyntax syntax, IType? type = null)
        => ImplicitElementAccess(type ?? (model.GetTypeInfo(syntax).Type is ITypeSymbol symbol
                ? Type(symbol) : TypeShorthands.VoidType),
            Parse(syntax.ArgumentList).Select(x => x.ToExpression()).ToList());

    public ExpressionCollection Parse(ImplicitArrayCreationExpressionSyntax syntax, IType? type = null)
        => AbstractCodeModelFactory.Expressions(Parse(syntax.Initializer, type ?? Type(model.GetTypeInfo(syntax))).Expressions);

    public IExpression Parse(ElementBindingExpressionSyntax syntax, IType? type = null)
        => throw new NotImplementedException();    // TODO

    public AnyArgExpression<ExpressionSyntax> Parse(ElementAccessExpressionSyntax syntax, IType? type = null)
        => AnyArgExpression(List(ParseExpression(syntax.Expression)).Concat(Parse(syntax.ArgumentList).Select(x => x.Value)).ToList(), OperationType.Bracket);

    public UnaryExpression Parse(DefaultExpressionSyntax syntax, IType? type = null)
        => UnaryExpression(Parse(syntax.Type), OperationType.Default);

    public IExpression Parse(DeclarationExpressionSyntax syntax, IType? type = null)
        => throw new NotImplementedException();

    public TernaryExpression Parse(ConditionalExpressionSyntax syntax, IType? type = null)
        => TernaryExpression(ParseExpression(syntax.Condition), ParseExpression(syntax.WhenTrue), ParseExpression(syntax.WhenFalse));

    public BinaryExpression Parse(ConditionalAccessExpressionSyntax syntax, IType? type = null)
        => BinaryExpression(ParseExpression(syntax.Expression), OperationType.ConditionalAccess, ParseExpression(syntax.WhenNotNull));

    public IExpression Parse(CheckedExpressionSyntax syntax, IType? type = null)
        => throw new NotImplementedException();

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
        ObjectCreationExpressionSyntax expression => Parse(expression, type ?? (SymbolUtils.GetDeclaration(syntax, model) is ISymbol symbol && SymbolUtils.IsNewDefined(symbol) ? GetModel<IClassDeclaration>(symbol).Get_Type() : Type(model.GetTypeInfo(syntax)))),
        _ => throw new NotImplementedException()
    };

    public ImplicitObjectCreationExpression Parse(ImplicitObjectCreationExpressionSyntax syntax, IType type)
        => ImplicitObjectCreation(type, Parse(syntax.ArgumentList).Select(x => x.Expression).ToList(), syntax.Initializer is null ? null : Parse(syntax.Initializer, type));

    public ObjectCreationExpression Parse(ObjectCreationExpressionSyntax syntax, IType type)
    {
        var symbol = model.GetSymbolInfo(syntax).Symbol;
        return ObjectCreation(type, syntax.ArgumentList is null ? null
            : Parse(syntax.ArgumentList).Select(x => x.Expression).ToList(),
            syntax.Initializer is null ? null : Parse(syntax.Initializer, GetObjectCreationType(syntax, type)), model.GetOperation(syntax));
    }

    public IType GetObjectCreationType(ObjectCreationExpressionSyntax syntax, IType type) => syntax switch
    {
        // (syntax.Type as GenericNameSyntax).TypeArgumentList.Arguments
        _ when syntax.Type is GenericNameSyntax genericName && genericName.Identifier.ToString() == "Dictionary" => ParseType(genericName.TypeArgumentList.Arguments.Last()),
        _ => type
    };

    public AwaitExpression Parse(AwaitExpressionSyntax syntax, IType? type = null) => Await(ParseExpression(syntax.Expression));

    public IExpression Parse(AssignmentExpressionSyntax syntax, IType? type = null)
        => Assignment(ParseExpression(syntax.Left, type), AssignmentTypeExtensions.GetAssignmentType(syntax.Kind()), ParseExpression(syntax.Right));

    public ArrayCreationExpression Parse(ArrayCreationExpressionSyntax syntax, IType? type = null)
    => ArrayCreation(type ?? Parse(syntax.Type),
        syntax.Type.RankSpecifiers.Any() ? syntax.Type.RankSpecifiers.Select(Parse).ToArray() : null,
        syntax.Initializer is null ? null : Parse(syntax.Initializer, type ?? Parse(syntax.Type)));

    public List<IExpression> Parse(ArrayRankSpecifierSyntax syntax)
        => syntax.Sizes.Select(x => ParseExpression(x)).ToList();

    public IExpression Parse(AnonymousObjectCreationExpressionSyntax syntax, IType? type = null)
         => throw new NotImplementedException();    // TODO

    public IAnonymousFunctionExpression Parse(AnonymousFunctionExpressionSyntax syntax, IType? type = null) => syntax switch
    {
        AnonymousMethodExpressionSyntax expression => Parse(expression),
        LambdaExpressionSyntax expression => Parse(expression),
        _ => throw new ArgumentException($"Can't parse {nameof(AnonymousObjectCreationExpressionSyntax)} from '{syntax}'.")
    };

    public AnonymousMethodExpression Parse(AnonymousMethodExpressionSyntax syntax)
         => AnonymousMethod(ParseModifier(syntax.Modifiers), syntax.AsyncKeyword != default, syntax.DelegateKeyword != default,
             syntax.ParameterList is null ? AbstractCodeModelFactory.NamedValues() : AbstractCodeModelParsing.ParseProperties(this, syntax.ParameterList), Parse(model.GetTypeInfo(syntax)),
             syntax.Block is null ? null : Parse(syntax.Block),
             syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody));

    public ILambdaExpression Parse(LambdaExpressionSyntax syntax, IType? type = null) => syntax switch
    {
        SimpleLambdaExpressionSyntax expression => Parse(expression),
        ParenthesizedLambdaExpressionSyntax expression => Parse(expression),
        _ => throw new ArgumentException($"Can't parse {nameof(LambdaExpressionSyntax)} from '{syntax}'.")
    };

    public SimpleLambdaExpression Parse(SimpleLambdaExpressionSyntax syntax)
         => SimpleLambda(Parse(syntax.Parameter), Parse(model.GetTypeInfo(syntax)),
             syntax.Block is null
                     ? syntax.ExpressionBody is null
                         ? null
                         : ParseExpression(syntax.ExpressionBody)
                     : Parse(syntax.Block), syntax.AsyncKeyword != default,
                 ParseModifier(syntax.Modifiers), MethodBodyPreference.Automatic);

    public ParenthesizedLambdaExpression Parse(ParenthesizedLambdaExpressionSyntax syntax)
         => ParenthesizedLambda(AbstractCodeModelParsing.ParseProperties(this, syntax.ParameterList), Parse(model.GetTypeInfo(syntax)),
             syntax.Block is null
                     ? syntax.ExpressionBody is null
                         ? null
                         : ParseExpression(syntax.ExpressionBody)
                     : Parse(syntax.Block), syntax.AsyncKeyword != default,
                 ParseModifier(syntax.Modifiers), MethodBodyPreference.Automatic);

    public IExpression Parse(LiteralExpressionSyntax syntax, IType? type = null) => syntax.Kind() switch
    {
        SyntaxKind.ArgListExpression => NullValue,
        SyntaxKind.NumericLiteralExpression => Literal(ParseNumber(syntax)),
        SyntaxKind.StringLiteralExpression => CodeModelFactory.Literal(syntax.Token.ValueText),
        SyntaxKind.CharacterLiteralExpression => CodeModelFactory.Literal(syntax.Token.ValueText[0]),
        SyntaxKind.TrueLiteralExpression => Literal(true),
        SyntaxKind.FalseLiteralExpression => Literal(false),
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
        string s when s.Contains('.') => double.Parse(token.Token.ValueText),
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

    public Block Parse(BlockSyntax syntax) => Block(List(syntax.Statements.Select(Parse)));
    public BreakStatement Parse(BreakStatementSyntax _) => Break();
    public CheckedStatement Parse(CheckedStatementSyntax syntax) => Checked(Parse(syntax.Block));
    public ForEachStatement Parse(CommonForEachStatementSyntax syntax) => syntax switch
    {
        ForEachStatementSyntax statement => Parse(statement),
        _ => throw new ArgumentException($"Can't parse {nameof(ForEachStatement)} from '{syntax}'.")
    };
    public ForEachStatement Parse(ForEachStatementSyntax syntax) => ForEach(ParseType(syntax.Type), syntax.Identifier.ToString(), ParseExpression(syntax.Expression), Parse(syntax.Statement));
    public ContinueStatement Parse(ContinueStatementSyntax _) => Continue();
    public DoStatement Parse(DoStatementSyntax syntax) => Do(Parse(syntax.Statement), ParseExpression(syntax.Condition));
    public EmptyStatement Parse(EmptyStatementSyntax _) => Empty();
    public ExpressionStatement Parse(ExpressionStatementSyntax syntax) => Statement(ParseExpression(syntax.Expression));
    public IMember Parse(GlobalStatementSyntax syntax)
    {
        var statement = Parse(syntax.Statement);
        return statement is ExpressionStatement ? statement : statement;
    }
    public FixedStatement Parse(FixedStatementSyntax syntax) => Fixed(Parse(syntax.Declaration), Parse(syntax.Statement));
    public VariableDeclarations Parse(VariableDeclarationSyntax syntax) => VariableDeclarations(ParseType(syntax.Type), Parse(syntax.Variables));
    public VariableDeclarator Parse(VariableDeclaratorSyntax syntax) => VariableDeclarator(syntax.Identifier.ToString(), syntax.Initializer is null ? null : ParseExpression(syntax.Initializer.Value));
    public List<VariableDeclarator> Parse(IEnumerable<VariableDeclaratorSyntax> syntax) => syntax.Select(Parse).ToList();
    public List<AbstractProperty> Parse(BracketedArgumentListSyntax syntax) => syntax.Arguments.Select(Parse).ToList();
    public AbstractProperty Parse(ArgumentSyntax syntax) => AbstractCodeModelFactory.NamedValue(TypeShorthands.VoidType, syntax.NameColon?.ToString(), ParseExpression(syntax.Expression));  // TODO: Semantics for type
    public Argument ParseToArgument(ArgumentSyntax syntax) => Arg(syntax.NameColon?.ToString(), ParseExpression(syntax.Expression));
    public List<Argument> Parse(ArgumentListSyntax syntax) => syntax.Arguments.Select(ParseToArgument).ToList();
    public List<AbstractProperty> Parse(IEnumerable<ArgumentSyntax> syntax) => syntax.Select(Parse).ToList();

    public AttributeList Parse(AttributeListSyntax syntax)
        => Attributes(syntax.Target is null ? null : Parse(syntax.Target), syntax.Attributes.Select(Parse).ToList());
    public AttributeListList Parse(IEnumerable<AttributeListSyntax> syntax)
        => AttributesList(syntax.Select(Parse).ToList());
    public AttributeTargetSpecifier Parse(AttributeTargetSpecifierSyntax syntax)
        => AttributeTargetSpecifier(syntax.Identifier.ToString());
    public Models.Primitives.Attribute.Attribute Parse(AttributeSyntax syntax)
        => Attribute(ParseType(syntax.Name), syntax.ArgumentList is null ? null : Parse(syntax.ArgumentList));
    public AttributeArgumentList Parse(AttributeArgumentListSyntax syntax)
        => AttributeArgs(syntax.Arguments.Select(Parse).ToList());
    public AttributeArgument Parse(AttributeArgumentSyntax syntax)
        => AttributeArg(ParseExpression(syntax.Expression), syntax.NameColon?.Name.ToString());
    public NameEquals Parse(NameEqualsSyntax syntax) => CodeModelFactory.NameEquals(syntax.Name.ToString());
    public NameColon Parse(NameColonSyntax syntax) => CodeModelFactory.NameColon(syntax.Name.ToString());

    public ForStatement Parse(ForStatementSyntax syntax)
        => For(syntax.Declaration is null ? null : Parse(syntax.Declaration),
            syntax.Initializers.Select(x => ParseExpression(x)).ToList(),
            ParseExpression(syntax.Condition),
            List(syntax.Incrementors.Select(x => ParseExpression(x))),
            Parse(syntax.Statement));
    public GotoStatement Parse(GotoStatementSyntax syntax) => Goto(ParseExpression(syntax.Expression));
    public IfStatement Parse(IfStatementSyntax syntax)
        => If(ParseExpression(syntax.Condition), Parse(syntax.Statement), syntax.Else is null ? null : Parse(syntax.Else));
    public IStatement Parse(ElseClauseSyntax syntax) => Parse(syntax.Statement);
    public LabeledStatement Parse(LabeledStatementSyntax syntax) => Labeled(syntax.Identifier.ToString(), Parse(syntax.Statement));
    public LocalDeclarationStatements Parse(LocalDeclarationStatementSyntax syntax) => LocalDeclarations(Parse(syntax.Declaration));
    public LocalFunctionStatement Parse(LocalFunctionStatementSyntax syntax)

         => Register(syntax, LocalFunction(syntax.GetName(),
                 AbstractCodeModelParsing.ParseProperties(this, syntax.ParameterList),
                 ParseType(syntax.ReturnType),
                 syntax.TypeParameterList?.Parameters.Select(Parse),
                 syntax.ConstraintClauses.Select(Parse).ToArray(),
                 syntax.Body is null
                     ? syntax.ExpressionBody is null
                         ? null
                         : ParseExpression(syntax.ExpressionBody.Expression)
                     : Parse(syntax.Body),
                 ParseModifier(syntax.Modifiers),
                 Parse(syntax.AttributeLists),
                 MethodBodyPreference.Automatic));

    public List<TypeParameterConstraintClause> Parse(IEnumerable<TypeParameterConstraintClauseSyntax> syntax) => syntax.Select(Parse).ToList();
    public TypeParameterConstraintClause Parse(TypeParameterConstraintClauseSyntax syntax)
        => ConstraintClause(syntax.Name.ToString(), syntax.Constraints.Select(Parse).ToList());
    public ITypeParameterConstraint Parse(TypeParameterConstraintSyntax syntax) => syntax switch
    {
        ClassOrStructConstraintSyntax constraint => Parse(constraint),
        ConstructorConstraintSyntax constraint => Parse(constraint),
        DefaultConstraintSyntax constraint => Parse(constraint),
        TypeConstraintSyntax constraint => Parse(constraint),
        _ => throw new ArgumentException($"Can't parse {nameof(ITypeParameterConstraint)} from '{syntax}'.")
    };
    public ClassOrStructConstraint Parse(ClassOrStructConstraintSyntax syntax) => ConstraintClassOrStruct(syntax.Kind(), syntax.ClassOrStructKeyword);
    public ConstructorConstraint Parse(ConstructorConstraintSyntax _) => ConstraintConstructor();
    public DefaultConstraint Parse(DefaultConstraintSyntax _) => ConstraintDefault();
    public TypeConstraint Parse(TypeConstraintSyntax syntax) => ConstraintType(ParseType(syntax.Type));
    public TypeCollection ParseTypes(TypeParameterListSyntax? syntax) => syntax is null ? AbstractCodeModelParsing.Types() : AbstractCodeModelParsing.Types(syntax.Parameters.Select(Parse));
    public AbstractProperty Parse(ParameterSyntax syntax) => AbstractCodeModelParsing.AbstractProperty(this, syntax);
    public IType Parse(TypeParameterSyntax syntax) => QuickType(syntax.Identifier.ToString());    // TODO
    public LockStatement Parse(LockStatementSyntax syntax) => Lock(ParseExpression(syntax.Expression), Parse(syntax.Statement));
    public ReturnStatement Parse(ReturnStatementSyntax syntax) => Return(ParseExpression(syntax.Expression));
    public SwitchStatement Parse(SwitchStatementSyntax syntax) => Switch(ParseExpression(syntax.Expression), List(syntax.Sections.Select(Parse)));
    public SwitchSection Parse(SwitchSectionSyntax syntax)
        => Cases(syntax.Labels.Select(Parse).ToList(),
           syntax.Statements.Select(Parse).ToList());
    public ISwitchLabel Parse(SwitchLabelSyntax syntax) => syntax switch
    {
        CasePatternSwitchLabelSyntax label => Parse(label),
        CaseSwitchLabelSyntax label => Parse(label),
        DefaultSwitchLabelSyntax _ => ParseDefaultSwitch(),
        _ => throw new NotImplementedException($"SwitchLabelSyntax {syntax} not implemented.")
    };
    public ThrowStatement Parse(ThrowStatementSyntax syntax) => Throw(ParseExpression(syntax.Expression));
    public TryStatement Parse(TryStatementSyntax syntax)
        => TryStatement(Parse(syntax.Block), List(syntax.Catches.Select(Parse)), syntax.Finally is null ? null : Parse(syntax.Finally));
    public CatchClause Parse(CatchClauseSyntax syntax)
        => Catch(syntax.Declaration is null ? TypeShorthands.VoidType : ParseType(syntax.Declaration.Type), syntax.Declaration?.Identifier.ToString(), Parse(syntax.Block), syntax.Filter is null ? null : Parse(syntax.Filter));
    public CatchFilterClause Parse(CatchFilterClauseSyntax syntax)
        => CatchFilter(ParseExpression(syntax.FilterExpression));
    public CatchDeclaration Parse(CatchDeclarationSyntax syntax)
        => CodeModelFactory.CatchDeclaration(ParseType(syntax.Type), syntax.Identifier.ToString());
    public FinallyClause Parse(FinallyClauseSyntax syntax) => Finally(Parse(syntax.Block));
    public UnsafeStatement Parse(UnsafeStatementSyntax syntax) => Unsafe(Parse(syntax.Block));
    public UsingStatement Parse(UsingStatementSyntax syntax)
        => Using(Parse(syntax.Statement),
            syntax.Declaration is null ? null : Parse(syntax.Declaration),
            syntax.Expression is null ? null : ParseExpression(syntax.Expression));

    public WhileStatement Parse(WhileStatementSyntax syntax)
        => While(ParseExpression(syntax.Condition), Parse(syntax.Statement));

    public CompilationUnit Parse() => Parse(CompilationUnit);
    public CompilationUnit Parse(CompilationUnitSyntax syntax)
        => CompilationUnit(syntax.Members.Select(Parse).ToList(), syntax.Usings.Select(Parse).ToList(), AttributesList(syntax.AttributeLists.Select(Parse)));
    public UsingDirective Parse(UsingDirectiveSyntax syntax)
            => UsingDir(syntax.Name?.ToString() ?? throw new NotImplementedException(),
                isGlobal: syntax.GlobalKeyword.IsKind(SyntaxKind.StaticKeyword),
                isStatic: syntax.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword),
                alias: syntax.Alias?.ToString());

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
        => Register(syntax, CodeModelFactory.Field(ParseType(syntax.Declaration.Type),
            Parse(syntax.Declaration).First().Name,
            syntax.Declaration.Variables.FirstOrDefault()?.Initializer is null ? null : ParseExpression(syntax.Declaration.Variables.First().Initializer!.Value),
            AttributesList(syntax.AttributeLists.Select(Parse)),
            ParseModifier(syntax.Modifiers)));

    public Field Parse(IFieldSymbol symbol)
    {
        VariableDeclaratorSyntax? variableDeclaratorSyntax = null;
        //VariableDeclarations? declarations = null;
        foreach (var declaring in symbol.DeclaringSyntaxReferences)
        {
            variableDeclaratorSyntax = declaring.GetSyntax() as VariableDeclaratorSyntax;
        }
        return Register(symbol, Field(Parse(symbol.Type),
            symbol.Name,
            //symbol.GetAttributes().Select(x => Parse(x)).ToList(),
            variableDeclaratorSyntax?.Initializer is null ? null : ParseExpression(variableDeclaratorSyntax.Initializer.Value),
            modifier: ParseModifier(symbol)));
    }

    public IMember Parse(IPropertySymbol symbol)
        => Parse(symbol.DeclaringSyntaxReferences.First().GetSyntax() as PropertyDeclarationSyntax ?? throw new NotImplementedException());

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
       => model.GetSymbolInfo(syntax).Symbol is IObjectCreationOperation objectCreationSymbol
        && SemanticReflection.GetConstructor(objectCreationSymbol) is ConstructorInfo constructorInfo
        ? CodeModelsFromReflection.Constructor(constructorInfo)
        : CodeModelFactory.Constructor(Parse(SymbolUtils.GetType(syntax, model) ?? throw new NotImplementedException()), AbstractCodeModelParsing.NamedValues(this, syntax),
            syntax.Body is null ? null : Parse(syntax.Body), syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody.Expression));
    public IMember Parse(ConversionOperatorDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(DestructorDeclarationSyntax syntax) => throw new NotImplementedException();
    public Method Parse(MethodDeclarationSyntax syntax)
         => Register(syntax, Method(syntax.GetName(),
                 AbstractCodeModelParsing.NamedValues(this, syntax),
                 ParseType(syntax.ReturnType),
                 syntax.TypeParameterList?.Parameters.Select(Parse),
                 syntax.ConstraintClauses.Select(Parse).ToArray(),
                 syntax.Body is null
                     ? syntax.ExpressionBody is null
                         ? null
                         : ParseExpression(syntax.ExpressionBody.Expression)
                     : Parse(syntax.Body),
                 ParseModifier(syntax.Modifiers),
                 Parse(syntax.AttributeLists),
                 MethodBodyPreference.Automatic));
    public IMember Parse(OperatorDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(BaseNamespaceDeclarationSyntax syntax) => syntax switch
    {
        FileScopedNamespaceDeclarationSyntax declaration => Parse(declaration),
        NamespaceDeclarationSyntax declaration => Parse(declaration),
        _ => throw new NotImplementedException($"Not implemented BaseNamespaceDeclaration: '{syntax}'.")
    };
    public IMember Parse(FileScopedNamespaceDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(NamespaceDeclarationSyntax syntax) => throw new NotImplementedException();
    public Namespace ParseToNamespace(NamespaceDeclarationSyntax syntax) => Namespace(syntax.Name.ToString());
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
        => Register(syntax, Property(ParseType(syntax.Type), syntax.Name(),
             syntax.ExpressionBody is not null
                ? Accessors(Accessor(AccessorType.Get, expressionBody: ParseExpression(syntax.ExpressionBody.Expression), modifier: ParseModifier(syntax.Modifiers)))
                : Parse(syntax.AccessorList!),
            syntax.Initializer is null ? null : ParseExpression(syntax.Initializer.Value),
            ParseModifier(syntax.Modifiers),
            AttributesList(syntax.AttributeLists.Select(Parse))));
    public AccessorList Parse(AccessorListSyntax syntax)
        => Accessors(syntax.Accessors.Select(Parse));
    public Accessor Parse(AccessorDeclarationSyntax syntax)
        => Models.Primitives.Member.Accessor.Create(AccessorTypeExtensions.FromSyntax(syntax.Kind()), syntax.Body is null ? null : Parse(syntax.Body),
           syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody?.Expression), modifier: ParseModifier(syntax.Modifiers));
    public IMember Parse(BaseTypeDeclarationSyntax syntax) => syntax switch
    {
        EnumDeclarationSyntax declaration => Parse(declaration),
        TypeDeclarationSyntax declaration => Parse(declaration),
        _ => throw new NotImplementedException($"Not implemented BaseTypeDeclaration: '{syntax}'.")
    };
    public IMember Parse(EnumDeclarationSyntax syntax, NamespaceDeclarationSyntax? @namespace = null) => Register(syntax, Enum(syntax.Identifier.ValueText,
           syntax.GetMembers().Select(x => (Parse(x) as IEnumMember)!).ToArray(),
    @namespace: @namespace == default ? default : ParseToNamespace(@namespace),
           modifier: ParseModifier(syntax.Modifiers),
           attributes: Parse(syntax.AttributeLists)));
    public IMember Parse(TypeDeclarationSyntax syntax) => syntax switch
    {
        ClassDeclarationSyntax declaration => Parse(declaration),
        InterfaceDeclarationSyntax declaration => Parse(declaration),
        RecordDeclarationSyntax declaration => Parse(declaration),
        StructDeclarationSyntax declaration => Parse(declaration),
        _ => throw new NotImplementedException($"Not implemented BaseTypeDeclaration: '{syntax}'.")
    };
    public ClassDeclaration Parse(ClassDeclarationSyntax syntax, NamespaceDeclarationSyntax? @namespace = null) =>
       Register(syntax, Class(syntax.Identifier.ValueText,
           syntax.TypeParameterList?.Parameters.Select(Parse),
           syntax.ConstraintClauses.Select(Parse).ToArray(),
           syntax.BaseList?.Types.Select(Parse).ToArray(),
           syntax.GetMembers().Select(Parse).ToArray(),
           @namespace: @namespace == default ? default : ParseToNamespace(@namespace),
           modifier: ParseModifier(syntax.Modifiers),
           attributes: Parse(syntax.AttributeLists)));

    public IMember Parse(InterfaceDeclarationSyntax syntax, NamespaceDeclarationSyntax? @namespace = null)
        => Register(syntax, Interface(syntax.Identifier.ValueText,
           syntax.TypeParameterList?.Parameters.Select(Parse),
           syntax.ConstraintClauses.Select(Parse).ToArray(),
           syntax.BaseList?.Types.Select(Parse).ToArray(),
           syntax.GetMembers().Select(Parse).ToArray(),
           @namespace: @namespace == default ? default : ParseToNamespace(@namespace),
           modifier: ParseModifier(syntax.Modifiers),
           attributes: Parse(syntax.AttributeLists)));

    public IMember Parse(RecordDeclarationSyntax syntax) => syntax switch
    {
        _ when syntax.IsKind(SyntaxKind.RecordDeclaration) => ParseRecordNonStruct(syntax),
        _ when syntax.IsKind(SyntaxKind.RecordStructDeclaration) => ParseRecordStruct(syntax),
        _ => throw new NotImplementedException($"Not implemented RecordDeclaration: '{syntax}'.")
    };

    public IMember ParseRecordNonStruct(RecordDeclarationSyntax syntax, NamespaceDeclarationSyntax? @namespace = null)
        => Register(syntax, Record(syntax.Identifier.ValueText,
           syntax.TypeParameterList?.Parameters.Select(Parse),
           syntax.ConstraintClauses.Select(Parse).ToArray(),
           syntax.BaseList?.Types.Select(Parse).ToArray(),
           (syntax.ParameterList?.Parameters.Select(ParseRecordParameter).ToArray() ?? Array.Empty<IMember>())
            .Concat(syntax.GetMembers().Select(Parse).ToArray()),
    @namespace: @namespace == default ? default : ParseToNamespace(@namespace),
           modifier: ParseModifier(syntax.Modifiers),
           attributes: Parse(syntax.AttributeLists)));
    public IMember ParseRecordParameter(ParameterSyntax syntax)
    {
        var parameter = Parse(syntax);
        return Property(parameter.Type, parameter.Name, Accessors(Accessor(AccessorType.Get), Accessor(AccessorType.Init)),
            parameter.Value,
            PropertyAndFieldTypes.RecordProperty,
            parameter.Attributes);
    }

    public IMember ParseRecordStruct(RecordDeclarationSyntax syntax)
        => throw new NotImplementedException($"Not implemented RecordDeclaration: '{syntax}'.");
    public IMember Parse(StructDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(DelegateDeclarationSyntax syntax) => throw new NotImplementedException();
    public IMember Parse(EnumMemberDeclarationSyntax syntax) => EnumField(syntax.Identifier.ValueText,
        syntax.EqualsValue is null ? null : ParseExpression(syntax.EqualsValue.Value),
        syntax.EqualsValue is null);
    public IMember Parse(IncompleteMemberSyntax syntax) => throw new NotImplementedException();

    public IBaseType Parse(BaseTypeSyntax syntax) => syntax switch
    {
        SimpleBaseTypeSyntax type => Parse(type),
        PrimaryConstructorBaseTypeSyntax type => Parse(type),
        _ => throw new NotImplementedException($"Not implemented BaseTypeSyntax: '{syntax}'.")
    };

    public SimpleBaseType Parse(SimpleBaseTypeSyntax syntax) => SimpleBase(ParseType(syntax.Type));
    public PrimaryConstructorBaseType Parse(PrimaryConstructorBaseTypeSyntax syntax)
        => PrimaryConstructorBase(ParseType(syntax.Type), List(syntax.ArgumentList.Arguments.Select(ParseToArgument)));
}
