using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using CodeAnalyzation.Models.ProgramModels;
using CodeAnalyzation.Parsing;
using CodeAnalyzation.Reflection;
using Generator.Models.Primitives.Expression.AnonymousFunction;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public static class CodeModelParsing
{
    private static IProgramContext Context => ProgramContext.Context!;
    private static T Register<T>(SyntaxNode node, T model) where T : ICodeModel => Context.Register(node, model);
    public static IType Parse(SyntaxToken token, SemanticModel? model = null) => ParseType(token.ToString(), model);
    public static IType ParseType(string identifier, SemanticModel? model = null) => ParseType(ParseTypeName(identifier), model: model);
    public static IType Parse(Microsoft.CodeAnalysis.TypeInfo typeInfo, SemanticModel? model = null) => typeInfo.Type is null && typeInfo.ConvertedType is null ? ParseType(typeInfo.ToString(), model) : Parse(typeInfo.Type ?? typeInfo.ConvertedType);  // TODO: Nullability
    public static TypeFromSymbol Parse(ITypeSymbol symbol) => new(symbol);

    public static IType ParseType(TypeSyntax? syntax, bool required = true, IType? knownType = null, SemanticModel? model = null)
    {
        var expression = Parse(syntax, required, knownType, model);
        return (expression is IType type ? type
            : expression is IdentifierExpression identifier
                ? new QuickType(identifier.Name) : expression.Get_Type()) ??
             throw new NotImplementedException($"No type for: '{syntax}'.");
    }
    public static IExpression Parse(TypeSyntax? syntax, bool required = true, IType? knownType = null, SemanticModel? model = null) => syntax switch
    {
        ArrayTypeSyntax type => Parse(type, knownType, model),
        FunctionPointerTypeSyntax type => Parse(type, knownType, model),
        NameSyntax type => Parse(type, knownType, model),
        NullableTypeSyntax type => Parse(type, knownType, model),
        OmittedTypeArgumentSyntax type => Parse(type, model),
        PointerTypeSyntax type => Parse(type, knownType, model),
        PredefinedTypeSyntax type => Parse(type, required, knownType, model),
        RefTypeSyntax type => Parse(type, knownType, model),
        TupleTypeSyntax type => Parse(type, knownType, model),
        null => TypeShorthands.NullType,
        _ => throw new NotImplementedException($"TypeSyntax {syntax} not implemented.")
    };

    public static IType Parse(ArrayTypeSyntax syntax, IType? type = null, SemanticModel? model = null) => QuickType.ArrayType(ParseType(syntax.ElementType, model: model));
    public static IType Parse(FunctionPointerTypeSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();
    public static IExpression Parse(NameSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax switch
    {
        AliasQualifiedNameSyntax name => Parse(name, type, model),
        QualifiedNameSyntax name => Parse(name, type, model),
        SimpleNameSyntax name => Parse(name, type, model),
        _ => throw new NotImplementedException($"NameSyntax {syntax} not implemented.")
    };
    public static IType Parse(AliasQualifiedNameSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();
    public static IType Parse(QualifiedNameSyntax syntax, IType? type = null, SemanticModel? model = null)
        => new TypeFromReflection(model is null
            ? System.Type.GetType(syntax.ToString())
            : SemanticReflection.GetType(model.GetTypeInfo(syntax).Type));
    public static IExpression Parse(SimpleNameSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax switch
    {
        GenericNameSyntax name => Parse(name, type, model),
        IdentifierNameSyntax name => Parse(name, type, model),
        _ => throw new NotImplementedException($"NameSyntax {syntax} not implemented.")
    };
    public static IType Parse(GenericNameSyntax syntax, IType? type = null, SemanticModel? model = null) => new QuickType(syntax.ToString());
    public static IExpression Parse(IdentifierNameSyntax syntax, IType? type = null, SemanticModel? model = null)
    {
        var symbol = model is null ? null : model.GetSymbolInfo(syntax).Symbol;
        return model.GetSymbolInfo(syntax).Symbol switch
        {
            IFieldSymbol field => new PropertyFromField(field).AccessValue(syntax.ToString(), type, field),
            IPropertySymbol property => new PropertyFromReflection(property).AccessValue(syntax.ToString(), type, property),
            //IMethodSymbol method => new MethodFromSymbol(method).Invoke(syntax.ToString(), type, method),
            ITypeSymbol typeSymbol => Parse(typeSymbol),
            INamespaceSymbol namespaceSymbol => new Namespace(namespaceSymbol),
            _ when syntax.IsKind(SyntaxKind.IdentifierName) => new IdentifierExpression(syntax.ToString(), type, symbol),
            _ when model.GetTypeInfo(syntax) is Microsoft.CodeAnalysis.TypeInfo typeInfo => Parse(typeInfo),
            _ => new QuickType(syntax.Identifier.ToString())
        };
    }

    public static IType Parse(NullableTypeSyntax syntax, IType? type = null, SemanticModel? model = null) => ParseType(syntax.ElementType, false);
    public static IType Parse(OmittedTypeArgumentSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IType Parse(PointerTypeSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();
    public static IType Parse(PredefinedTypeSyntax syntax, bool required, IType? type = null, SemanticModel? model = null)
        => new QuickType(syntax.Keyword.ToString(), required);
    public static IType Parse(RefTypeSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();
    public static IType Parse(TupleTypeSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();

    public static Modifier ParseModifier(SyntaxTokenList syntax) => Modifier.None.SetModifiers(syntax.Select(ParseSingleModifier));
    public static Modifier ParseSingleModifier(SyntaxToken syntax) => syntax.Kind() switch
    {
        SyntaxKind.PrivateKeyword => Modifier.Private,
        SyntaxKind.ProtectedKeyword => Modifier.Protected,
        SyntaxKind.InternalKeyword => Modifier.Internal,
        SyntaxKind.PublicKeyword => Modifier.Public,
        SyntaxKind.ReadOnlyKeyword => Modifier.Readonly,
        SyntaxKind.ConstKeyword => Modifier.Const,
        SyntaxKind.AbstractKeyword => Modifier.Abstract,
        _ => throw new ArgumentException($"Unhandled token '{syntax}'.")
    };

    public static Property ParseProperty(ArgumentSyntax syntax, IType? specifiedType = null, SemanticModel? model = null) => syntax.Expression switch
    {
        TypeSyntax type => new(Parse(type), syntax.NameColon?.Name.ToString()),
        DeclarationExpressionSyntax declaration => ParseProperty(declaration, specifiedType, model: model),
        ExpressionSyntax expression => new(ParseExpression(expression, model: model)),
        _ => throw new ArgumentException($"Can't parse {nameof(Property)} from '{syntax}'.")
    };

    public static Property ParseProperty(DeclarationExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => new(Parse(syntax.Type), syntax.Designation switch
    {
        null => default,
        SingleVariableDesignationSyntax designation => designation.Identifier.ToString(),
        _ => throw new ArgumentException($"Can't parse {nameof(Property)} from '{syntax}'.")
    });

    public static PropertyCollection ParsePropertyCollection(string code) => code.Parse(code).Members.FirstOrDefault() switch
    {
        ClassDeclarationSyntax declaration => new(declaration),
        RecordDeclarationSyntax declaration => new(declaration),
        GlobalStatementSyntax statement => ParsePropertyCollection(statement),
        _ => throw new ArgumentException($"Can't parse {nameof(PropertyCollection)} from '{code}'.")
    };

    public static PropertyCollection ParsePropertyCollection(GlobalStatementSyntax syntax) => syntax.Statement switch
    {
        ExpressionStatementSyntax expression => ParsePropertyCollection(expression.Expression),
        _ => throw new ArgumentException($"Can't parse {nameof(PropertyCollection)} from '{syntax}'.")
    };

    public static PropertyCollection ParsePropertyCollection(ExpressionSyntax syntax, IType? type = null) => syntax switch
    {
        TupleExpressionSyntax declaration => ParsePropertyCollection(declaration.Arguments, nameByIndex: true),
        TupleTypeSyntax declaration => new PropertyCollection(declaration),
        _ => throw new ArgumentException($"Can't parse {nameof(PropertyCollection)} from '{syntax}'.")
    };

    public static PropertyCollection ParsePropertyCollection(IType Type, IEnumerable<ExpressionSyntax> syntax, bool nameByIndex = false, SemanticModel? model = null)
        => new(syntax.Select((x, i) => new Property(Type, nameByIndex ? $"Item{i + 1}" : null, ParseExpression(x, Type, model))), specifiedType: Type);

    public static PropertyCollection ParsePropertyCollection(IEnumerable<ArgumentSyntax> arguments, bool nameByIndex = false, IType? type = null, SemanticModel? model = null)
        => new(arguments.Select((x, i) => nameByIndex ? x.NameColon is null ? x.WithNameColon(NameColon($"Item{i + 1}")) : x : x).Select(x => ParseProperty(x, type, model)), specifiedType: type);

    public static PropertyCollection ParsePropertyCollection(ArgumentListSyntax syntax, IType? type = null, SemanticModel? model = null) => ParsePropertyCollection(syntax.Arguments, type: type, model: model);

    public static IExpression ParseExpression(ExpressionSyntax? syntax, IType? type = null, SemanticModel? model = null) => syntax switch
    {
        null => VoidValue,
        AnonymousFunctionExpressionSyntax expression => Parse(expression, type, model: model),
        AnonymousObjectCreationExpressionSyntax expression => Parse(expression, type, model: model),
        ArrayCreationExpressionSyntax expression => Parse(expression, type, model: model),
        AssignmentExpressionSyntax expression => Parse(expression, type, model),
        AwaitExpressionSyntax expression => Parse(expression, type, model),
        BaseObjectCreationExpressionSyntax expression => Parse(expression, type, model),
        BinaryExpressionSyntax expression => Parse(expression, type, model),
        CastExpressionSyntax expression => Parse(expression, type, model),
        CheckedExpressionSyntax expression => Parse(expression, type, model),
        ConditionalAccessExpressionSyntax expression => Parse(expression, type, model),
        ConditionalExpressionSyntax expression => Parse(expression, type, model),
        DeclarationExpressionSyntax expression => Parse(expression, type, model),
        DefaultExpressionSyntax expression => Parse(expression, type, model),
        ElementAccessExpressionSyntax expression => Parse(expression, type, model),
        ElementBindingExpressionSyntax expression => Parse(expression, type, model),
        ImplicitArrayCreationExpressionSyntax expression => Parse(expression, type, model),
        ImplicitElementAccessSyntax expression => Parse(expression, type, model),
        ImplicitStackAllocArrayCreationExpressionSyntax expression => Parse(expression, type, model: model),
        InitializerExpressionSyntax expression => Parse(expression, type ?? TypeShorthands.NullType, model: model),
        InstanceExpressionSyntax expression => Parse(expression, type, model: model),
        InterpolatedStringExpressionSyntax expression => Parse(expression, type),
        InvocationExpressionSyntax expression => Parse(expression, type, model),
        IsPatternExpressionSyntax expression => Parse(expression, type, model),
        LiteralExpressionSyntax expression => Parse(expression, type, model),
        MakeRefExpressionSyntax expression => Parse(expression, type, model),
        MemberAccessExpressionSyntax expression => Parse(expression, type, model),
        MemberBindingExpressionSyntax expression => Parse(expression, type),
        OmittedArraySizeExpressionSyntax expression => Parse(expression, type),
        ParenthesizedExpressionSyntax expression => Parse(expression, type, model),
        PostfixUnaryExpressionSyntax expression => Parse(expression, type, model: model),
        PrefixUnaryExpressionSyntax expression => Parse(expression, type),
        QueryExpressionSyntax expression => Parse(expression, type, model),
        RangeExpressionSyntax expression => Parse(expression, type, model),
        RefExpressionSyntax expression => Parse(expression, type, model),
        RefTypeExpressionSyntax expression => Parse(expression, type, model),
        RefValueExpressionSyntax expression => Parse(expression, type, model),
        SizeOfExpressionSyntax expression => Parse(expression, type, model),
        StackAllocArrayCreationExpressionSyntax expression => Parse(expression, type),
        SwitchExpressionSyntax expression => Parse(expression, type, model),
        ThrowExpressionSyntax expression => Parse(expression, type, model),
        TupleExpressionSyntax expression => Parse(expression, type, model),
        TypeOfExpressionSyntax expression => Parse(expression, type, model),
        TypeSyntax expression => Parse(expression, true, type, model),
        WithExpressionSyntax expression => Parse(expression, type, model),
        _ => throw new NotImplementedException()
    };

    public static BinaryExpression Parse(WithExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Expression, model: model), OperationType.With, Parse(syntax.Initializer, type ?? TypeShorthands.NullType, model));
    public static TypeOfExpression Parse(TypeOfExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => new(type ?? ParseType(syntax.Type, model: model));
    public static PropertyCollection Parse(TupleExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => ParsePropertyCollection(syntax.Arguments, nameByIndex: true, model: model);
    public static ThrowExpression Parse(ThrowExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => new(ParseExpression(syntax.Expression, model: model));

    public static IExpression Parse(SwitchExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();    // TODO

    public static IExpression Parse(StackAllocArrayCreationExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();    // TODO

    public static SizeOfExpression Parse(SizeOfExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => new(type ?? ParseType(syntax.Type, model: model));

    public static UnaryExpression Parse(RefValueExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Expression, model: model), OperationType.Ref);

    public static UnaryExpression Parse(RefTypeExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();    // TODO

    public static IExpression Parse(RefExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Expression, model: model), OperationType.Ref);

    public static IExpression Parse(RangeExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();    // TODO

    public static IExpression Parse(QueryExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();    // TODO

    public static UnaryExpression Parse(PrefixUnaryExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax.Kind() switch
    {
        SyntaxKind.UnaryPlusExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand, model: model), OperationType.UnaryAdd),
        SyntaxKind.UnaryMinusExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand, model: model), OperationType.UnarySubtract),
        SyntaxKind.BitwiseNotExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand, model: model), OperationType.Complement),
        SyntaxKind.LogicalNotExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand, model: model), OperationType.Not),
        SyntaxKind.PreIncrementExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand, model: model), OperationType.UnaryAddBefore),
        SyntaxKind.PreDecrementExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand, model: model), OperationType.UnarySubtractBefore),
        SyntaxKind.AddressOfExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand, model: model), OperationType.AddressOf),
        SyntaxKind.PointerIndirectionExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand, model: model), OperationType.PointerIndirection),
        SyntaxKind.IndexExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand, model: model), OperationType.Index),
        _ => throw new NotImplementedException()
    };

    public static UnaryExpression Parse(PostfixUnaryExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax.Kind() switch
    {
        SyntaxKind.PostIncrementExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand, model: model), OperationType.UnaryAddAfter),
        SyntaxKind.PostDecrementExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand, model: model), OperationType.UnarySubtractAfter),
        SyntaxKind.SuppressNullableWarningExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand, model: model), OperationType.SuppressNullableWarning),
        _ => throw new NotImplementedException()
    };

    public static UnaryExpression Parse(ParenthesizedExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Expression, model: model), OperationType.Parenthesis);
    public static IExpression Parse(OmittedArraySizeExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static IExpression Parse(MemberBindingExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static MemberAccessExpression Parse(MemberAccessExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax.Kind() switch
    {
        SyntaxKind.SimpleMemberAccessExpression => new(ParseExpression(syntax.Expression, model: model), Parse(syntax.Name, model: model).GetIdentifier()),
        SyntaxKind.PointerMemberAccessExpression => throw new NotImplementedException($"PointerMemberAccessExpression not implemented: {syntax}"),
        _ => throw new NotImplementedException($"MemberAccessExpression not implemented: {syntax}")
    };

    public static IExpression Parse(MakeRefExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => throw new NotImplementedException();    // TODO

    public static PatternExpression Parse(IsPatternExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => CodeModelFactory.PatternExpression(Parse(syntax.Pattern, model), ParseExpression(syntax.Expression, model: model));

    public static IPattern Parse(PatternSyntax syntax, SemanticModel? model = null) => syntax switch
    {
        BinaryPatternSyntax pattern => Parse(pattern, model),
        ConstantPatternSyntax pattern => Parse(pattern, model),
        DeclarationPatternSyntax pattern => Parse(pattern, model),
        DiscardPatternSyntax pattern => Parse(pattern, model),
        ParenthesizedPatternSyntax pattern => Parse(pattern, model),
        RecursivePatternSyntax pattern => Parse(pattern, model),
        RelationalPatternSyntax pattern => Parse(pattern, model),
        TypePatternSyntax pattern => Parse(pattern, model),
        UnaryPatternSyntax pattern => Parse(pattern, model),
        VarPatternSyntax pattern => Parse(pattern, model),
        _ => throw new NotImplementedException($"Pattern not implemented: {syntax}")
    };


    public static BinaryPattern Parse(BinaryPatternSyntax syntax, SemanticModel? model = null) => syntax.Kind() switch
    {
        SyntaxKind.OrPattern => new OrPattern(Parse(syntax.Left, model), Parse(syntax.Right, model)),
        SyntaxKind.AndPattern => new AndPattern(Parse(syntax.Left, model), Parse(syntax.Right, model)),
        _ => throw new NotImplementedException($"Not implemented BinaryPattern: '{syntax}'.")
    };
    public static ConstantPattern Parse(ConstantPatternSyntax syntax, SemanticModel? model = null)
        => new(ParseExpression(syntax.Expression, model: model));
    public static DeclarationPattern Parse(DeclarationPatternSyntax syntax, SemanticModel? model = null)
        => new(ParseType(syntax.Type, model: model), Parse(syntax.Designation, model));
    public static DiscardPattern Parse(DiscardPatternSyntax syntax, SemanticModel? model = null)
        => new();
    public static ParenthesizedPattern Parse(ParenthesizedPatternSyntax syntax, SemanticModel? model = null)
        => new(Parse(syntax.Pattern, model));
    public static RecursivePattern Parse(RecursivePatternSyntax syntax, SemanticModel? model = null)
        => new(ParseType(syntax.Type, model: model));
    public static RelationalPattern Parse(RelationalPatternSyntax syntax, SemanticModel? model = null)
        => new(syntax.OperatorToken, ParseExpression(syntax.Expression, model: model));
    public static TypePattern Parse(TypePatternSyntax syntax, SemanticModel? model = null)
        => new(ParseType(syntax.Type, model: model));
    public static UnaryPattern Parse(UnaryPatternSyntax syntax, SemanticModel? model = null)
        => new(Parse(syntax.Pattern, model: model));
    public static VarPattern Parse(VarPatternSyntax syntax, SemanticModel? model = null)
        => new(Parse(syntax.Designation, model: model));
    public static IVariableDesignation Parse(VariableDesignationSyntax syntax, SemanticModel? model = null) => syntax switch
    {
        DiscardDesignationSyntax designation => Parse(designation, model),
        SingleVariableDesignationSyntax designation => Parse(designation, model),
        ParenthesizedVariableDesignationSyntax designation => Parse(designation, model),
        _ => throw new NotImplementedException($"Not implemented VariableDesignation: '{syntax}'.")
    };
    public static DiscardDesignation Parse(DiscardDesignationSyntax syntax, SemanticModel? model = null)
        => new();
    public static SingleVariableDesignation Parse(SingleVariableDesignationSyntax syntax, SemanticModel? model = null)
        => new(syntax.Identifier.ToString());
    public static ParenthesizedVariableDesignation Parse(ParenthesizedVariableDesignationSyntax syntax, SemanticModel? model = null)
        => new(syntax.Variables.Select(x => Parse(x, model)).ToList());

    public static CasePatternSwitchLabel Parse(CasePatternSwitchLabelSyntax syntax, SemanticModel? model = null)
        => new(Parse(syntax.Pattern, model), syntax.WhenClause is null ? null : Parse(syntax.WhenClause, model));

    public static CaseSwitchLabel Parse(CaseSwitchLabelSyntax syntax, SemanticModel? model = null)
        => new(ParseExpression(syntax.Value, model: model));

    public static DefaultSwitchLabel Parse()
        => new();

    public static WhenClause Parse(WhenClauseSyntax syntax, SemanticModel? model = null)
        => new(ParseExpression(syntax.Condition, model: model));

    public static InvocationFromReflection Parse(InvocationExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
    {
        if (model is null)
        {
            throw new ArgumentException($"No Semantic model. Check the stack where it may have gone.");
        }
        var symbol = model.GetSymbolInfo(syntax);
        var methodSymbol = (symbol.Symbol ?? symbol.CandidateSymbols.FirstOrDefault()) as IMethodSymbol;
        if (methodSymbol is null)
        {
            throw new ArgumentException($"No Symbol found for {syntax}. Probably Assembly isn't loaded.");
        }
        var methodInfo = SemanticReflection.GetMethod(methodSymbol);
        if (methodInfo is null)
        {
            throw new ArgumentException($"Couldn't find method for symbol: {methodSymbol}");
        }
        var expression = ParseExpression(syntax.Expression, model: model);
        var caller = expression is MemberAccessExpression access ? access.Expression : expression;
        var argumentExpressions = ParsePropertyCollection(syntax.ArgumentList, model: model).ToExpressions();
        var argumentList = methodSymbol.MethodKind is MethodKind.ReducedExtension
            ? new IExpression[] { caller }.Concat(argumentExpressions).ToList()
            : argumentExpressions;
        return new InvocationFromReflection(methodInfo, caller, argumentList);
    }

    public static IExpression Parse(InterpolatedStringExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();    // TODO

    public static IExpression Parse(InstanceExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax switch
    {
        BaseExpressionSyntax expression => Parse(expression, type, model),
        ThisExpressionSyntax expression => Parse(expression, type, model),
        _ => throw new NotImplementedException()
    };

    public static IExpression Parse(BaseExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax.Kind() switch
    {
        SyntaxKind.BaseExpression => new IdentifierExpression(syntax.Token.ToString()), // IDK, TODO
        _ => throw new NotImplementedException()
    };
    public static IExpression Parse(ThisExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();    // TODO

    public static InitializerExpression Parse(InitializerExpressionSyntax syntax, IType Type, SemanticModel? model = null) => syntax.Kind() switch
    {
        SyntaxKind.ObjectInitializerExpression => new(Type, syntax.Kind(), ParsePropertyCollection(Type, syntax.Expressions, model: model)),
        SyntaxKind.CollectionInitializerExpression => new(Type, syntax.Kind(), ParsePropertyCollection(Type, syntax.Expressions, model: model)),
        SyntaxKind.ArrayInitializerExpression => Type.Identifier switch
        {
            "Dictionary" or "IDictionary" => new(Type, syntax.Kind(), ParsePropertyCollection(Type.GetGenericType(1), syntax.Expressions, model: model)),
            _ => new(Type, syntax.Kind(), ParsePropertyCollection(Type, syntax.Expressions, model: model)),
        },
        SyntaxKind.ComplexElementInitializerExpression => new(Type, syntax.Kind(), ParsePropertyCollection(Type, syntax.Expressions, model: model)),
        SyntaxKind.WithInitializerExpression => new(Type, syntax.Kind(), ParsePropertyCollection(Type, syntax.Expressions, model: model)),
        _ => throw new NotImplementedException()
    };

    public static IExpression Parse(ImplicitStackAllocArrayCreationExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();    // TODO

    public static ImplicitElementAccessExpression Parse(ImplicitElementAccessSyntax syntax, IType? type = null, SemanticModel? model = null)
        => new(type ?? (model.GetTypeInfo(syntax).Type is ITypeSymbol symbol ? Type(symbol) : TypeShorthands.VoidType), Parse(syntax.ArgumentList).Select(x => x.ToExpression()).ToList());

    public static ExpressionCollection Parse(ImplicitArrayCreationExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => Parse(syntax.Initializer, type ?? Type(model.GetTypeInfo(syntax)), model).Expressions.ToValueCollection();

    public static IExpression Parse(ElementBindingExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();    // TODO

    public static AnyArgExpression<ExpressionSyntax> Parse(ElementAccessExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => AnyArgExpression(List(ParseExpression(syntax.Expression, model: model)).Concat(Parse(syntax.ArgumentList).Select(x => x.Value)).ToList(), OperationType.Bracket);

    public static UnaryExpression Parse(DefaultExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => CodeModelFactory.UnaryExpression(Parse(syntax.Type, model: model), OperationType.Default);

    public static IExpression Parse(DeclarationExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();

    public static TernaryExpression Parse(ConditionalExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => CodeModelFactory.TernaryExpression(ParseExpression(syntax.Condition, model: model), ParseExpression(syntax.WhenTrue, model: model), ParseExpression(syntax.WhenFalse, model: model));

    public static BinaryExpression Parse(ConditionalAccessExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Expression, model: model), OperationType.ConditionalAccess, ParseExpression(syntax.WhenNotNull, model: model));

    public static IExpression Parse(CheckedExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => throw new NotImplementedException();

    public static UnaryExpression Parse(CastExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Expression, model: model), OperationType.Cast);

    public static BinaryExpression Parse(BinaryExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax.Kind() switch
    {
        SyntaxKind.AddExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.Plus, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.SubtractExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.Subtract, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.MultiplyExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.Multiply, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.DivideExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.Divide, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.ModuloExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.Modulo, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.LeftShiftExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.LeftShift, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.RightShiftExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.RightShift, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.LogicalOrExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.LogicalOr, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.LogicalAndExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.LogicalAnd, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.BitwiseOrExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.BitwiseOr, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.BitwiseAndExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.BitwiseAnd, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.ExclusiveOrExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.ExclusiveOr, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.EqualsExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.Equals, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.NotEqualsExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.NotEquals, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.LessThanExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.LessThan, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.LessThanOrEqualExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.LessThanOrEqual, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.GreaterThanExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.GreaterThan, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.GreaterThanOrEqualExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.GreaterThanOrEqual, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.IsExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.Is, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.AsExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.As, ParseExpression(syntax.Right, model: model)),
        SyntaxKind.CoalesceExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left, model: model), OperationType.Coalesce, ParseExpression(syntax.Right, model: model)),
        _ => throw new NotImplementedException()
    };

    public static IExpression Parse(BaseObjectCreationExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax switch
    {
        ImplicitObjectCreationExpressionSyntax expression => Parse(expression, type ?? Type(model.GetTypeInfo(syntax)), model),
        ObjectCreationExpressionSyntax expression => Parse(expression, type ?? Type(model.GetTypeInfo(syntax)), model),
        _ => throw new NotImplementedException()
    };

    public static ImplicitObjectCreationExpression Parse(ImplicitObjectCreationExpressionSyntax syntax, IType type, SemanticModel? model = null)
        => new(type, ParsePropertyCollection(syntax.ArgumentList), syntax.Initializer is null ? null : Parse(syntax.Initializer, type));
    public static ObjectCreationExpression Parse(ObjectCreationExpressionSyntax syntax, IType type, SemanticModel? model = null)
    {
        var symbol = model?.GetSymbolInfo(syntax).Symbol;
        var op = model?.GetOperation(syntax);
        var t = model?.GetTypeInfo(syntax);
        return new(type, syntax.ArgumentList is null ? null : ParsePropertyCollection(syntax.ArgumentList, GetObjectCreationType(syntax, type), model),
            syntax.Initializer is null ? null : Parse(syntax.Initializer, GetObjectCreationType(syntax, type), model), model?.GetOperation(syntax));
    }

    public static IType GetObjectCreationType(ObjectCreationExpressionSyntax syntax, IType type) => syntax switch
    {
        // (syntax.Type as GenericNameSyntax).TypeArgumentList.Arguments
        _ when syntax.Type is GenericNameSyntax genericName && genericName.Identifier.ToString() == "Dictionary" => ParseType(genericName.TypeArgumentList.Arguments.Last()),
        _ => type
    };

    public static AwaitExpression Parse(AwaitExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => new(ParseExpression(syntax.Expression, model: model));

    public static IExpression Parse(AssignmentExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax.Kind() switch
    {
        SyntaxKind.SimpleAssignmentExpression => new SimpleAssignmentExpression(ParseExpression(syntax.Left, type, model), ParseExpression(syntax.Right, model: model)),
        SyntaxKind.AddAssignmentExpression => new AddAssignmentExpression(ParseExpression(syntax.Left, model: model), ParseExpression(syntax.Right, model: model)),
        SyntaxKind.SubtractAssignmentExpression => new SubtractAssignmentExpression(ParseExpression(syntax.Left, model: model), ParseExpression(syntax.Right, model: model)),
        SyntaxKind.MultiplyAssignmentExpression => new MultiplyAssignmentExpression(ParseExpression(syntax.Left, model: model), ParseExpression(syntax.Right, model: model)),
        SyntaxKind.DivideAssignmentExpression => new DivideAssignmentExpression(ParseExpression(syntax.Left, model: model), ParseExpression(syntax.Right, model: model)),
        SyntaxKind.ModuloAssignmentExpression => new ModuloAssignmentExpression(ParseExpression(syntax.Left, model: model), ParseExpression(syntax.Right, model: model)),
        SyntaxKind.AndAssignmentExpression => new AndAssignmentExpression(ParseExpression(syntax.Left, model: model), ParseExpression(syntax.Right, model: model)),
        SyntaxKind.ExclusiveOrAssignmentExpression => new ExclusiveOrAssignmentExpression(ParseExpression(syntax.Left, model: model), ParseExpression(syntax.Right, model: model)),
        SyntaxKind.OrAssignmentExpression => new OrAssignmentExpression(ParseExpression(syntax.Left, model: model), ParseExpression(syntax.Right, model: model)),
        SyntaxKind.LeftShiftAssignmentExpression => new LeftShiftAssignmentExpression(ParseExpression(syntax.Left, model: model), ParseExpression(syntax.Right, model: model)),
        SyntaxKind.RightShiftAssignmentExpression => new RightShiftAssignmentExpression(ParseExpression(syntax.Left, model: model), ParseExpression(syntax.Right, model: model)),
        SyntaxKind.CoalesceAssignmentExpression => new CoalesceAssignmentExpression(ParseExpression(syntax.Left, model: model), ParseExpression(syntax.Right, model: model)),
        _ => throw new NotImplementedException($"Assignment expression {syntax} not implemented.")
    };

    public static ExpressionCollection Parse(ArrayCreationExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => Parse(syntax.Initializer, type ?? Parse(syntax.Type, model: model), model).Expressions.ToValueCollection();

    public static IExpression Parse(AnonymousObjectCreationExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
         => throw new NotImplementedException();    // TODO

    public static IAnonymousFunctionExpression Parse(AnonymousFunctionExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax switch
    {
        AnonymousMethodExpressionSyntax expression => Parse(expression, model),
        LambdaExpressionSyntax expression => Parse(expression, model: model),
        _ => throw new ArgumentException($"Can't parse {nameof(AnonymousObjectCreationExpressionSyntax)} from '{syntax}'.")
    };

    public static AnonymousMethodExpression Parse(AnonymousMethodExpressionSyntax syntax, SemanticModel? model = null)
         => new(ParseModifier(syntax.Modifiers), syntax.AsyncKeyword != default, syntax.DelegateKeyword != default,
             ParseProperties(syntax.ParameterList, model), Parse(model.GetTypeInfo(syntax)),
             syntax.Block is null ? null : Parse(syntax.Block, model),
             syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody, model: model));

    public static ILambdaExpression Parse(LambdaExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax switch
    {
        SimpleLambdaExpressionSyntax expression => Parse(expression, model),
        ParenthesizedLambdaExpressionSyntax expression => Parse(expression, model),
        _ => throw new ArgumentException($"Can't parse {nameof(LambdaExpressionSyntax)} from '{syntax}'.")
    };

    public static SimpleLambdaExpression Parse(SimpleLambdaExpressionSyntax syntax, SemanticModel? model = null)
         => new(ParseModifier(syntax.Modifiers), syntax.AsyncKeyword != default,
             Parse(syntax.Parameter, model), Parse(model.GetTypeInfo(syntax), model),
             syntax.Block is null ? null : Parse(syntax.Block, model),
             syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody, model: model));

    public static ParenthesizedLambdaExpression Parse(ParenthesizedLambdaExpressionSyntax syntax, SemanticModel? model = null)
         => new(ParseModifier(syntax.Modifiers), syntax.AsyncKeyword != default,
             ParseProperties(syntax.ParameterList, model), Parse(model.GetTypeInfo(syntax)),
             syntax.Block is null ? null : Parse(syntax.Block, model),
             syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody, model: model));

    public static LiteralExpression Parse(LiteralExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => syntax.Kind() switch
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

    public static object ParseNumber(LiteralExpressionSyntax token) => token.Token.ToString().ToUpper() switch
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
            > long.MaxValue => (object)decimal.Parse(token.Token.ValueText),
            > int.MaxValue => (object)long.Parse(token.Token.ValueText),
            _ => (object)int.Parse(token.Token.ValueText),
        },
        _ => decimal.Parse(token.Token.ValueText) switch
        {
            // CASTS ARE NEEDED
            > ulong.MaxValue => (object)decimal.Parse(token.Token.ValueText),
            > long.MaxValue => (object)ulong.Parse(token.Token.ValueText),
            > uint.MaxValue => (object)long.Parse(token.Token.ValueText),
            > int.MaxValue => (object)uint.Parse(token.Token.ValueText),
            _ => (object)int.Parse(token.Token.ValueText),
        },
    };

    public static IStatement Parse(StatementSyntax syntax, SemanticModel? model = null) => syntax switch
    {
        BlockSyntax statement => Parse(statement, model),
        BreakStatementSyntax statement => Parse(statement),
        CheckedStatementSyntax statement => Parse(statement),
        CommonForEachStatementSyntax statement => Parse(statement, model),
        ContinueStatementSyntax statement => Parse(statement),
        DoStatementSyntax statement => Parse(statement, model),
        EmptyStatementSyntax statement => Parse(statement),
        ExpressionStatementSyntax statement => Parse(statement, model),
        FixedStatementSyntax statement => Parse(statement),
        ForStatementSyntax statement => Parse(statement, model),
        GotoStatementSyntax statement => Parse(statement, model),
        IfStatementSyntax statement => Parse(statement, model),
        LabeledStatementSyntax statement => Parse(statement, model),
        LocalDeclarationStatementSyntax statement => Parse(statement, model),
        LocalFunctionStatementSyntax statement => Parse(statement, model),
        LockStatementSyntax statement => Parse(statement, model),
        ReturnStatementSyntax statement => Parse(statement, model),
        SwitchStatementSyntax statement => Parse(statement, model),
        ThrowStatementSyntax statement => Parse(statement, model),
        TryStatementSyntax statement => Parse(statement, model),
        UnsafeStatementSyntax statement => Parse(statement, model),
        UsingStatementSyntax statement => Parse(statement, model),
        WhileStatementSyntax statement => Parse(statement, model),
        _ => throw new ArgumentException($"Can't parse {nameof(IStatement)} from '{syntax}'.")
    };

    public static Block Parse(BlockSyntax syntax, SemanticModel? model = null) => new(List(syntax.Statements.Select(x => Parse(x, model))));
    public static BreakStatement Parse(BreakStatementSyntax _) => new();
    public static CheckedStatement Parse(CheckedStatementSyntax syntax) => new(Parse(syntax.Block));
    public static ForEachStatement Parse(CommonForEachStatementSyntax syntax, SemanticModel? model = null) => syntax switch
    {
        ForEachStatementSyntax statement => Parse(statement, model),
        _ => throw new ArgumentException($"Can't parse {nameof(ForEachStatement)} from '{syntax}'.")
    };
    public static ForEachStatement Parse(ForEachStatementSyntax syntax, SemanticModel? model = null) => new(ParseType(syntax.Type, model: model), syntax.Identifier.ToString(), ParseExpression(syntax.Expression, model: model), Parse(syntax.Statement, model));
    public static ContinueStatement Parse(ContinueStatementSyntax _) => new();
    public static DoStatement Parse(DoStatementSyntax syntax, SemanticModel? model = null) => new(Parse(syntax.Statement, model), ParseExpression(syntax.Condition, model: model));
    public static EmptyStatement Parse(EmptyStatementSyntax _) => new();
    public static ExpressionStatement Parse(ExpressionStatementSyntax syntax, SemanticModel? model = null) => new(ParseExpression(syntax.Expression, model: model));
    public static IMember Parse(GlobalStatementSyntax syntax, SemanticModel? model = null)
    {
        var statement = Parse(syntax.Statement, model);
        return statement is ExpressionStatement ? statement : statement;
    }
    public static FixedStatement Parse(FixedStatementSyntax syntax, SemanticModel? model = null) => new(Parse(syntax.Declaration, model), Parse(syntax.Statement, model));
    public static VariableDeclarations Parse(VariableDeclarationSyntax syntax, SemanticModel? model = null) => new(ParseType(syntax.Type, model: model), Parse(syntax.Variables, model));
    public static VariableDeclarator Parse(VariableDeclaratorSyntax syntax, SemanticModel? model = null) => new(syntax.Identifier.ToString(), syntax.Initializer is null ? null : ParseExpression(syntax.Initializer.Value, model: model));
    public static List<VariableDeclarator> Parse(IEnumerable<VariableDeclaratorSyntax> syntax, SemanticModel? model = null) => syntax.Select(x => Parse(x, model)).ToList();
    public static List<Property> Parse(BracketedArgumentListSyntax syntax, SemanticModel? model = null) => syntax.Arguments.Select(x => Parse(x, model)).ToList();
    public static Property Parse(ArgumentSyntax syntax, SemanticModel? model = null) => new(TypeShorthands.VoidType, syntax.NameColon?.ToString(), ParseExpression(syntax.Expression, model: model));  // TODO: Semantics for type
    public static List<Property> Parse(IEnumerable<ArgumentSyntax> syntax, SemanticModel? model = null) => syntax.Select(x => Parse(x)).ToList();
    public static AttributeList Parse(AttributeListSyntax syntax, SemanticModel? model = null) => new(syntax.Target is null ? null : Parse(syntax.Target), syntax.Attributes.Select(Parse).ToList());
    public static AttributeTargetSpecifier Parse(AttributeTargetSpecifierSyntax syntax, SemanticModel? model = null) => new(syntax.Identifier.ToString());
    public static Attribute Parse(AttributeSyntax syntax)
        => new(syntax.Name.ToString(), new(syntax.ArgumentList is null ? new List<AttributeArgument>() : syntax.ArgumentList.Arguments.Select(Parse).ToList()));
    public static AttributeArgument Parse(AttributeArgumentSyntax syntax) => new(ParseExpression(syntax.Expression), syntax.NameColon?.Name.ToString());
    public static ForStatement Parse(ForStatementSyntax syntax, SemanticModel? model = null)
        => new(syntax.Declaration is null ? new(null) : Parse(syntax.Declaration, model), syntax.Initializers.Select(x => ParseExpression(x, model: model)).ToList(), ParseExpression(syntax.Condition, model: model), List(syntax.Incrementors.Select(x => ParseExpression(x, model: model))), Parse(syntax.Statement, model));
    public static GotoStatement Parse(GotoStatementSyntax syntax, SemanticModel? model = null) => new(ParseExpression(syntax.Expression, model: model));
    public static IfStatement Parse(IfStatementSyntax syntax, SemanticModel? model = null)
        => new(ParseExpression(syntax.Condition, model: model), Parse(syntax.Statement, model), syntax.Else is null ? null : Parse(syntax.Else, model));
    public static IStatement Parse(ElseClauseSyntax syntax, SemanticModel? model = null) => Parse(syntax.Statement, model);
    public static LabeledStatement Parse(LabeledStatementSyntax syntax, SemanticModel? model = null) => new(syntax.Identifier.ToString(), Parse(syntax.Statement, model));
    public static LocalDeclarationStatements Parse(LocalDeclarationStatementSyntax syntax, SemanticModel? model = null) => new(Parse(syntax.Declaration, model));
    public static LocalFunctionStatement Parse(LocalFunctionStatementSyntax syntax, SemanticModel? model = null)
        => new(ParseModifier(syntax.Modifiers), ParseType(syntax.ReturnType), syntax.Identifier.ToString(),
            ParseTypes(syntax.TypeParameterList),
            ParseProperties(syntax.ParameterList, model), Parse(syntax.ConstraintClauses), syntax.Body is null ? null : Parse(syntax.Body, model),
            syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody.Expression, model: model));
    public static PropertyCollection ParseProperties(ParameterListSyntax syntax, SemanticModel? model = null) => new(syntax.Parameters.Select(x => Parse(x, model)));
    public static List<TypeParameterConstraintClause> Parse(IEnumerable<TypeParameterConstraintClauseSyntax> syntax, SemanticModel? model = null) => syntax.Select(x => Parse(x)).ToList();
    public static TypeParameterConstraintClause Parse(TypeParameterConstraintClauseSyntax syntax, SemanticModel? model = null)
        => new(syntax.Name.ToString(), syntax.Constraints.Select(x => Parse(x, model)).ToList());
    public static ITypeParameterConstraint Parse(TypeParameterConstraintSyntax syntax, SemanticModel? model = null) => syntax switch
    {
        ClassOrStructConstraintSyntax constraint => Parse(constraint),
        ConstructorConstraintSyntax constraint => Parse(constraint),
        DefaultConstraintSyntax constraint => Parse(constraint),
        TypeConstraintSyntax constraint => Parse(constraint),
        _ => throw new ArgumentException($"Can't parse {nameof(ITypeParameterConstraint)} from '{syntax}'.")
    };
    public static ClassOrStructConstraint Parse(ClassOrStructConstraintSyntax syntax, SemanticModel? model = null) => new(syntax.Kind(), syntax.ClassOrStructKeyword);
    public static ConstructorConstraint Parse(ConstructorConstraintSyntax _, SemanticModel? model = null) => new();
    public static DefaultConstraint Parse(DefaultConstraintSyntax _, SemanticModel? model = null) => new();
    public static TypeConstraint Parse(TypeConstraintSyntax syntax, SemanticModel? model = null) => new(ParseType(syntax.Type, model: model));
    public static TypeCollection ParseTypes(TypeParameterListSyntax? syntax, SemanticModel? model = null) => syntax is null ? new() : new(syntax.Parameters.Select(x => Parse(x, model)));
    public static Property Parse(ParameterSyntax syntax, SemanticModel? model = null) => new(syntax);
    public static IType Parse(TypeParameterSyntax syntax, SemanticModel? model = null) => new QuickType(syntax.Identifier.ToString());    // TODO
    public static LockStatement Parse(LockStatementSyntax syntax, SemanticModel? model = null) => new(ParseExpression(syntax.Expression, model: model), Parse(syntax.Statement, model));
    public static ReturnStatement Parse(ReturnStatementSyntax syntax, SemanticModel? model = null) => new(ParseExpression(syntax.Expression, model: model));
    public static SwitchStatement Parse(SwitchStatementSyntax syntax, SemanticModel? model = null) => new(ParseExpression(syntax.Expression, model: model), List(syntax.Sections.Select(x => Parse(x))));
    public static SwitchSection Parse(SwitchSectionSyntax syntax, SemanticModel? model = null)
        => new(syntax.Labels.Select(x => Parse(x, model: model)).ToList(),
           syntax.Statements.Select(x => Parse(x, model)).ToList());
    public static ISwitchLabel Parse(SwitchLabelSyntax syntax, SemanticModel? model = null) => syntax switch
    {
        CasePatternSwitchLabelSyntax label => Parse(label, model),
        CaseSwitchLabelSyntax label => Parse(label, model),
        DefaultSwitchLabelSyntax _ => Parse(),
        _ => throw new NotImplementedException($"SwitchLabelSyntax {syntax} not implemented.")
    };
    public static ThrowStatement Parse(ThrowStatementSyntax syntax, SemanticModel? model = null) => new(ParseExpression(syntax.Expression, model: model));
    public static TryStatement Parse(TryStatementSyntax syntax, SemanticModel? model = null)
        => new(Parse(syntax.Block, model), List(syntax.Catches.Select(x => Parse(x, model))), syntax.Finally is null ? null : Parse(syntax.Finally, model));
    public static CatchClause Parse(CatchClauseSyntax syntax, SemanticModel? model = null)
        => new(syntax.Declaration is null ? TypeShorthands.VoidType : ParseType(syntax.Declaration.Type, model: model), syntax.Declaration?.Identifier.ToString(), Parse(syntax.Block, model));
    public static CatchDeclaration Parse(CatchDeclarationSyntax syntax, SemanticModel? model = null) => new(ParseType(syntax.Type), syntax.Identifier.ToString());
    public static FinallyClause Parse(FinallyClauseSyntax syntax, SemanticModel? model = null) => new(Parse(syntax.Block, model));
    public static UnsafeStatement Parse(UnsafeStatementSyntax syntax, SemanticModel? model = null) => new(Parse(syntax.Block, model));
    public static UsingStatement Parse(UsingStatementSyntax syntax, SemanticModel? model = null) => new(Parse(syntax.Statement, model));
    public static WhileStatement Parse(WhileStatementSyntax syntax, SemanticModel? model = null) => new(ParseExpression(syntax.Condition, model: model), Parse(syntax.Statement, model: model));

    public static CompilationUnit Parse(CompilationUnitSyntax syntax, SemanticModel model)
            => new(syntax.Members.Select(x => Parse(x, model)).ToList(), syntax.Usings.Select(Parse).ToList(), syntax.AttributeLists.Select(x => Parse(x, model)).ToList());
    public static UsingDirective Parse(UsingDirectiveSyntax syntax)
            => new(syntax.Name.ToString(), IsGlobal: syntax.GlobalKeyword.IsKind(SyntaxKind.StaticKeyword), IsStatic: syntax.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword), Alias: syntax.Alias?.ToString());

    public static IMember Parse(MemberDeclarationSyntax syntax, SemanticModel? model = null) => syntax switch
    {
        BaseFieldDeclarationSyntax declaration => Parse(declaration, model),
        BaseMethodDeclarationSyntax declaration => Parse(declaration, model),
        BaseNamespaceDeclarationSyntax declaration => Parse(declaration, model),
        BasePropertyDeclarationSyntax declaration => Parse(declaration, model),
        BaseTypeDeclarationSyntax declaration => Parse(declaration, model),
        DelegateDeclarationSyntax declaration => Parse(declaration, model),
        EnumMemberDeclarationSyntax declaration => Parse(declaration, model),
        GlobalStatementSyntax declaration => Parse(declaration, model),
        IncompleteMemberSyntax declaration => Parse(declaration, model),
        _ => throw new NotImplementedException($"Not implemented MemberDeclaration: '{syntax}'.")
    };
    public static IMember Parse(BaseFieldDeclarationSyntax syntax, SemanticModel? model = null) => syntax switch
    {
        EventFieldDeclarationSyntax declaration => Parse(declaration, model),
        FieldDeclarationSyntax declaration => Parse(declaration, model),
        _ => throw new NotImplementedException($"Not implemented BaseFieldDeclaration: '{syntax}'.")
    };
    public static IMember Parse(EventFieldDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IMember Parse(FieldDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IMember Parse(BaseMethodDeclarationSyntax syntax, SemanticModel? model = null) => syntax switch
    {
        ConstructorDeclarationSyntax declaration => Parse(declaration, model),
        ConversionOperatorDeclarationSyntax declaration => Parse(declaration, model),
        DestructorDeclarationSyntax declaration => Parse(declaration, model),
        MethodDeclarationSyntax declaration => Parse(declaration, model),
        OperatorDeclarationSyntax declaration => Parse(declaration, model),
        _ => throw new NotImplementedException($"Not implemented BaseMethodDeclaration: '{syntax}'.")
    };
    public static Constructor Parse(ConstructorDeclarationSyntax syntax, SemanticModel? model = null)
       => new(syntax.Identifier.ToString(), new PropertyCollection(syntax), syntax.Body is null ? null : Parse(syntax.Body, model), syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody.Expression, model: model));
    public static IMember Parse(ConversionOperatorDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IMember Parse(DestructorDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static Method Parse(MethodDeclarationSyntax syntax, SemanticModel? model = null)
         => Register(syntax, new Method(syntax.GetName(), new PropertyCollection(syntax), ParseType(syntax.ReturnType, model: model), syntax.Body is null ? null : Parse(syntax.Body, model), syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody.Expression, model: model)));
    public static IMember Parse(OperatorDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IMember Parse(BaseNamespaceDeclarationSyntax syntax, SemanticModel? model = null) => syntax switch
    {
        FileScopedNamespaceDeclarationSyntax declaration => Parse(declaration, model),
        NamespaceDeclarationSyntax declaration => Parse(declaration, model),
        _ => throw new NotImplementedException($"Not implemented BaseNamespaceDeclaration: '{syntax}'.")
    };
    public static IMember Parse(FileScopedNamespaceDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IMember Parse(NamespaceDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IMember Parse(BasePropertyDeclarationSyntax syntax, SemanticModel? model = null) => syntax switch
    {
        EventDeclarationSyntax declaration => Parse(declaration, model),
        IndexerDeclarationSyntax declaration => Parse(declaration, model),
        PropertyDeclarationSyntax declaration => Parse(declaration, model),
        _ => throw new NotImplementedException($"Not implemented BasePropertyDeclaration: '{syntax}'.")
    };
    public static IMember Parse(EventDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IMember Parse(IndexerDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IMember Parse(PropertyDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IMember Parse(BaseTypeDeclarationSyntax syntax, SemanticModel? model = null) => syntax switch
    {
        EnumDeclarationSyntax declaration => Parse(declaration, model),
        TypeDeclarationSyntax declaration => Parse(declaration, model),
        _ => throw new NotImplementedException($"Not implemented BaseTypeDeclaration: '{syntax}'.")
    };
    public static IMember Parse(EnumDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IMember Parse(TypeDeclarationSyntax syntax, SemanticModel? model = null) => syntax switch
    {
        ClassDeclarationSyntax declaration => Parse(declaration, model: model),
        InterfaceDeclarationSyntax declaration => Parse(declaration, model),
        RecordDeclarationSyntax declaration => Parse(declaration, model),
        StructDeclarationSyntax declaration => Parse(declaration, model),
        _ => throw new NotImplementedException($"Not implemented BaseTypeDeclaration: '{syntax}'.")
    };
    public static ClassModel Parse(ClassDeclarationSyntax @class, NamespaceDeclarationSyntax? @namespace = null, SemanticModel? model = null) =>
       Register<ClassModel>(@class, @class.IsStatic() ? new StaticClass(@class.Identifier.ValueText,
           null,
           @class.GetMethods().Select(x => Method(x, model)),
           @namespace: @namespace == default ? default : new(@namespace))
       : new InstanceClass(@class.Identifier.ValueText,
           null,
           @class.GetMethods().Select(x => Method(x, model)),
           @namespace: @namespace == default ? default : new(@namespace)));

    public static IMember Parse(InterfaceDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IMember Parse(RecordDeclarationSyntax syntax, SemanticModel? model = null) => syntax switch
    {
        _ when syntax.IsKind(SyntaxKind.RecordDeclaration) => ParseRecordNonStruct(syntax, model),
        _ when syntax.IsKind(SyntaxKind.RecordStructDeclaration) => ParseRecordStruct(syntax, model),
        _ => throw new NotImplementedException($"Not implemented RecordDeclaration: '{syntax}'.")
    };
    public static IMember ParseRecordNonStruct(RecordDeclarationSyntax syntax, SemanticModel? model = null)
        => throw new NotImplementedException($"Not implemented RecordDeclaration: '{syntax}'.");
    public static IMember ParseRecordStruct(RecordDeclarationSyntax syntax, SemanticModel? model = null)
        => throw new NotImplementedException($"Not implemented RecordDeclaration: '{syntax}'.");
    public static IMember Parse(StructDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IMember Parse(DelegateDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IMember Parse(EnumMemberDeclarationSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
    public static IMember Parse(IncompleteMemberSyntax syntax, SemanticModel? model = null) => throw new NotImplementedException();
}
