using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Parsing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public static class CodeModelParsing
{
    public static IType Parse(SyntaxToken token) => ParseType(token.ToString());
    public static IType ParseType(string code) => ParseType(ParseTypeName(code));

    // TODO: Handle sourceSyntax: fullType ?? t
    public static IType ParseType(TypeSyntax? type, bool required = true, TypeSyntax? fullType = null) => type switch
    {
        PredefinedTypeSyntax t => new QuickType(t.Keyword.ToString(), required),
        NullableTypeSyntax t => ParseType(t.ElementType, false, fullType: fullType ?? t),
        IdentifierNameSyntax t => new QuickType(t.Identifier.ToString()),
        ArrayTypeSyntax t => new QuickType(t.ElementType.ToString(), isMulti: true),
        GenericNameSyntax t => new QuickType(t.ToString()),
        null => TypeShorthands.NullType,
        _ => throw new ArgumentException($"Unhandled {nameof(TypeSyntax)}: '{type}'.")
    };

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

    public static Property ParseProperty(ArgumentSyntax syntax, IType? specifiedType = null) => syntax.Expression switch
    {
        TypeSyntax type => new(ParseType(type), syntax.NameColon?.Name.ToString()),
        DeclarationExpressionSyntax declaration => ParseProperty(declaration, specifiedType),
        _ => throw new ArgumentException($"Can't parse {nameof(Property)} from '{syntax}'.")
    };

    public static Property ParseProperty(DeclarationExpressionSyntax syntax, IType? type = null) => new(ParseType(syntax.Type), syntax.Designation switch
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

    public static PropertyCollection ParsePropertyCollection(IType Type, IEnumerable<ExpressionSyntax> syntax, bool nameByIndex = false)
        => new( syntax.Select((x, i) => new Property(Type, nameByIndex ? $"Item{i + 1}" : null, ParseExpression(x, Type))), specifiedType: Type);

    public static PropertyCollection ParsePropertyCollection(IEnumerable<ArgumentSyntax> arguments, bool nameByIndex = false, IType? type = null)
        => new(arguments.Select((x, i) => nameByIndex ? x.NameColon is null ? x.WithNameColon(NameColon($"Item{i + 1}")) : x : x).Select(x => ParseProperty(x, type)), specifiedType: type);

    public static PropertyCollection ParsePropertyCollection(ArgumentListSyntax syntax, IType? type = null) => ParsePropertyCollection(syntax.Arguments, type: type);


    public static IExpression ParseExpression(ExpressionSyntax? syntax, IType? type = null) => syntax switch
    {
        null => CodeModelFactory.VoidValue,
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
        ThrowExpressionSyntax expression => Parse(expression, type),
        TupleExpressionSyntax expression => Parse(expression, type),
        TypeOfExpressionSyntax expression => Parse(expression, type),
        TypeSyntax expression => Parse(expression, type),
        WithExpressionSyntax expression => Parse(expression, type),
        _ => throw new NotImplementedException()
    };

    public static BinaryExpression Parse(WithExpressionSyntax syntax, IType? type = null)
        => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Expression), OperationType.With, Parse(syntax.Initializer, type ?? TypeShorthands.NullType));
    public static UnaryExpression Parse(TypeOfExpressionSyntax syntax, IType? type = null)
        => CodeModelFactory.UnaryExpression(Parse(syntax.Type), OperationType.Typeof);
    public static PropertyCollection Parse(TupleExpressionSyntax syntax, IType? type = null) => ParsePropertyCollection(syntax.Arguments, nameByIndex: true);
    public static ThrowExpression Parse(ThrowExpressionSyntax syntax, IType? type = null) => new(ParseExpression(syntax.Expression));

    public static IExpression Parse(SwitchExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static IExpression Parse(StackAllocArrayCreationExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static UnaryExpression Parse(SizeOfExpressionSyntax syntax, IType? type = null)
        => CodeModelFactory.UnaryExpression(Parse(syntax.Type), OperationType.Sizeof);

    public static UnaryExpression Parse(RefValueExpressionSyntax syntax, IType? type = null)
        => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Expression), OperationType.Ref);

    public static UnaryExpression Parse(RefTypeExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static IExpression Parse(RefExpressionSyntax syntax, IType? type = null)
        => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Expression), OperationType.Ref);

    public static IExpression Parse(RangeExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static IExpression Parse(QueryExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static UnaryExpression Parse(PrefixUnaryExpressionSyntax syntax, IType? type = null) => syntax.Kind() switch
    {
        SyntaxKind.UnaryPlusExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand), OperationType.UnaryAdd),
        SyntaxKind.UnaryMinusExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand), OperationType.UnarySubtract),
        SyntaxKind.BitwiseNotExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand), OperationType.Complement),
        SyntaxKind.LogicalNotExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand), OperationType.Not),
        SyntaxKind.PreIncrementExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand), OperationType.UnaryAddBefore),
        SyntaxKind.PreDecrementExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand), OperationType.UnarySubtractBefore),
        SyntaxKind.AddressOfExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand), OperationType.AddressOf),
        SyntaxKind.PointerIndirectionExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand), OperationType.PointerIndirection),
        SyntaxKind.IndexExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand), OperationType.Index),
        _ => throw new NotImplementedException()
    };

    public static UnaryExpression Parse(PostfixUnaryExpressionSyntax syntax, IType? type = null) => syntax.Kind() switch
    {
        SyntaxKind.PostIncrementExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand), OperationType.UnaryAddAfter),
        SyntaxKind.PostDecrementExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand), OperationType.UnarySubtractAfter),
        SyntaxKind.SuppressNullableWarningExpression => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Operand), OperationType.SuppressNullableWarning),
        _ => throw new NotImplementedException()
    };

    public static UnaryExpression Parse(ParenthesizedExpressionSyntax syntax, IType? type = null)
        => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Expression), OperationType.Parenthesis);


    public static IExpression Parse(OmittedArraySizeExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static IExpression Parse(MemberBindingExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static MemberAccessExpression Parse(MemberAccessExpressionSyntax syntax, IType? type = null) => syntax.Kind() switch
    {
        SyntaxKind.SimpleMemberAccessExpression => new(ParseExpression(syntax.Expression), syntax.Name.ToString()),   // TODO: Type from semantic analysis
        SyntaxKind.PointerMemberAccessExpression => throw new NotImplementedException($"PointerMemberAccessExpression not implemented: {syntax}"),
        _ => throw new NotImplementedException($"MemberAccessExpression not implemented: {syntax}")
    };

    public static IExpression Parse(MakeRefExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static BinaryExpression Parse(IsPatternExpressionSyntax syntax, IType? type = null)
        => CodeModelFactory.BinaryExpression(Parse(syntax.Pattern), OperationType.Is, ParseExpression(syntax.Expression));
    public static IExpression Parse(PatternSyntax syntax) => syntax switch
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
    public static IExpression Parse(BinaryPatternSyntax syntax) => throw new NotImplementedException();    // TODO
    public static IExpression Parse(ConstantPatternSyntax syntax) => throw new NotImplementedException();    // TODO
    public static IExpression Parse(DeclarationPatternSyntax syntax) => throw new NotImplementedException();    // TODO
    public static IExpression Parse(DiscardPatternSyntax syntax) => throw new NotImplementedException();    // TODO
    public static IExpression Parse(ParenthesizedPatternSyntax syntax) => throw new NotImplementedException();    // TODO
    public static IExpression Parse(RecursivePatternSyntax syntax) => throw new NotImplementedException();    // TODO
    public static IExpression Parse(RelationalPatternSyntax syntax) => throw new NotImplementedException();    // TODO
    public static IExpression Parse(TypePatternSyntax syntax) => throw new NotImplementedException();    // TODO
    public static IExpression Parse(UnaryPatternSyntax syntax) => throw new NotImplementedException();    // TODO
    public static IExpression Parse(VarPatternSyntax syntax) => throw new NotImplementedException();    // TODO


    public static InvocationExpression Parse(InvocationExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static IExpression Parse(InterpolatedStringExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static IExpression Parse(InstanceExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static InitializerExpression Parse(InitializerExpressionSyntax syntax, IType Type) => syntax.Kind() switch
    {
        SyntaxKind.ObjectInitializerExpression => new(Type, syntax.Kind(), ParsePropertyCollection(Type, syntax.Expressions)),
        SyntaxKind.CollectionInitializerExpression => new(Type, syntax.Kind(), ParsePropertyCollection(Type, syntax.Expressions)),
        SyntaxKind.ArrayInitializerExpression => Type.Identifier switch
        {
            "Dictionary" => new(Type, syntax.Kind(), ParsePropertyCollection(Type.GetGenericType(1), syntax.Expressions)),
            "IDictionary" => new(Type, syntax.Kind(), ParsePropertyCollection(Type.GetGenericType(1), syntax.Expressions)),
            _ => new(Type, syntax.Kind(), ParsePropertyCollection(Type, syntax.Expressions)),
        },
        SyntaxKind.ComplexElementInitializerExpression => new(Type, syntax.Kind(), ParsePropertyCollection(Type, syntax.Expressions)),
        SyntaxKind.WithInitializerExpression => new(Type, syntax.Kind(), ParsePropertyCollection(Type, syntax.Expressions)),
        _ => throw new NotImplementedException()
    };

    public static IExpression Parse(ImplicitStackAllocArrayCreationExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static IExpression Parse(ImplicitElementAccessSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static IExpression Parse(ImplicitArrayCreationExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static IExpression Parse(ElementBindingExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static AnyArgExpression<ExpressionSyntax> Parse(ElementAccessExpressionSyntax syntax, IType? type = null)
        => CodeModelFactory.AnyArgExpression(new List<IExpression>() { ParseExpression(syntax.Expression) }.Concat(Parse(syntax.ArgumentList).Select(x => x.Value)).ToList(), OperationType.Bracket);

    public static UnaryExpression Parse(DefaultExpressionSyntax syntax, IType? type = null)
        => CodeModelFactory.UnaryExpression(Parse(syntax.Type), OperationType.Default);

    public static IExpression Parse(DeclarationExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();

    public static TernaryExpression Parse(ConditionalExpressionSyntax syntax, IType? type = null)
        => CodeModelFactory.TernaryExpression(ParseExpression(syntax.Condition), ParseExpression(syntax.WhenTrue), ParseExpression(syntax.WhenFalse));

    public static BinaryExpression Parse(ConditionalAccessExpressionSyntax syntax, IType? type = null)
        => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Expression), OperationType.ConditionalAccess, ParseExpression(syntax.WhenNotNull));

    public static IExpression Parse(CheckedExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();

    public static UnaryExpression Parse(CastExpressionSyntax syntax, IType? type = null)
        => CodeModelFactory.UnaryExpression(ParseExpression(syntax.Expression), OperationType.Cast);

    public static BinaryExpression Parse(BinaryExpressionSyntax syntax, IType? type = null) => syntax.Kind() switch
    {
        SyntaxKind.AddExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.ConditionalAccess, ParseExpression(syntax.Right)),
        SyntaxKind.SubtractExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.Subtract, ParseExpression(syntax.Right)),
        SyntaxKind.MultiplyExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.Multiply, ParseExpression(syntax.Right)),
        SyntaxKind.DivideExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.Divide, ParseExpression(syntax.Right)),
        SyntaxKind.ModuloExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.Modulo, ParseExpression(syntax.Right)),
        SyntaxKind.LeftShiftExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.LeftShift, ParseExpression(syntax.Right)),
        SyntaxKind.RightShiftExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.RightShift, ParseExpression(syntax.Right)),
        SyntaxKind.LogicalOrExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.LogicalOr, ParseExpression(syntax.Right)),
        SyntaxKind.LogicalAndExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.LogicalAnd, ParseExpression(syntax.Right)),
        SyntaxKind.BitwiseOrExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.BitwiseOr, ParseExpression(syntax.Right)),
        SyntaxKind.BitwiseAndExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.BitwiseAnd, ParseExpression(syntax.Right)),
        SyntaxKind.ExclusiveOrExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.ExclusiveOr, ParseExpression(syntax.Right)),
        SyntaxKind.EqualsExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.Equals, ParseExpression(syntax.Right)),
        SyntaxKind.NotEqualsExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.NotEquals, ParseExpression(syntax.Right)),
        SyntaxKind.LessThanExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.LessThan, ParseExpression(syntax.Right)),
        SyntaxKind.LessThanOrEqualExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.LessThanOrEqual, ParseExpression(syntax.Right)),
        SyntaxKind.GreaterThanExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.GreaterThan, ParseExpression(syntax.Right)),
        SyntaxKind.GreaterThanOrEqualExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.GreaterThanOrEqual, ParseExpression(syntax.Right)),
        SyntaxKind.IsExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.Is, ParseExpression(syntax.Right)),
        SyntaxKind.AsExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.As, ParseExpression(syntax.Right)),
        SyntaxKind.CoalesceExpression => CodeModelFactory.BinaryExpression(ParseExpression(syntax.Left), OperationType.Coalesce, ParseExpression(syntax.Right)),
        _ => throw new NotImplementedException()
    };

    public static IExpression Parse(BaseObjectCreationExpressionSyntax syntax, IType? type = null) => syntax switch
    {
        ImplicitObjectCreationExpressionSyntax expression => Parse(expression, type),
        ObjectCreationExpressionSyntax expression => Parse(expression, type ?? TypeShorthands.NullType),    // TODO: Get type from semantic analysis
        _ => throw new NotImplementedException()
    };

    public static ImplicitObjectCreationExpression Parse(ImplicitObjectCreationExpressionSyntax syntax, IType type)
        => new(type, ParsePropertyCollection(syntax.ArgumentList), syntax.Initializer is null ? null : Parse(syntax.Initializer, type));
    public static ObjectCreationExpression Parse(ObjectCreationExpressionSyntax syntax, IType type)
        => new(type, syntax.ArgumentList is null ? null : ParsePropertyCollection(syntax.ArgumentList, GetObjectCreationType(syntax, type)), syntax.Initializer is null ? null : Parse(syntax.Initializer, GetObjectCreationType(syntax, type)));

    public static IType GetObjectCreationType(ObjectCreationExpressionSyntax syntax, IType type) => syntax switch
    {
        // (syntax.Type as GenericNameSyntax).TypeArgumentList.Arguments
        _ when syntax.Type is GenericNameSyntax genericName && genericName.Identifier.ToString() == "Dictionary" => ParseType(genericName.TypeArgumentList.Arguments.Last()),
        _ => type
    };

    public static AwaitExpression Parse(AwaitExpressionSyntax syntax, IType? type = null) => new(ParseExpression(syntax.Expression));

    public static IExpression Parse(AssignmentExpressionSyntax syntax, IType? type = null) => syntax.Kind() switch
    {
        SyntaxKind.SimpleAssignmentExpression => new SimpleAssignmentExpression(ParseExpression(syntax.Left), ParseExpression(syntax.Right)),
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

    public static ExpressionCollection Parse(ArrayCreationExpressionSyntax syntax, IType? type = null)
        => Parse(syntax.Initializer, type ?? ParseType(syntax.Type)).Expressions.ToValueCollection();

    public static IExpression Parse(AnonymousObjectCreationExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static TypeExpression Parse(TypeSyntax syntax, IType? type = null) => new(type ?? ParseType(syntax));

    public static IExpression Parse(AnonymousFunctionExpressionSyntax syntax, IType? type = null) => throw new NotImplementedException();    // TODO

    public static LiteralExpression Parse(LiteralExpressionSyntax syntax, IType? type = null) => syntax.Kind() switch
    {
        SyntaxKind.ArgListExpression => CodeModelFactory.NullValue,
        SyntaxKind.NumericLiteralExpression => new(ParseNumber(syntax.Token.ValueText)),
        SyntaxKind.StringLiteralExpression => new(syntax.Token.ValueText),
        SyntaxKind.CharacterLiteralExpression => new(syntax.Token.ValueText[0]),
        SyntaxKind.TrueLiteralExpression => new(true),
        SyntaxKind.FalseLiteralExpression => new(false),
        SyntaxKind.NullLiteralExpression => CodeModelFactory.NullValue,
        SyntaxKind.DefaultLiteralExpression => CodeModelFactory.DefaultValue,
        _ => throw new ArgumentException($"Unhandled literal kind '{syntax}'.")
    };

    public static object ParseNumber(string s) => s switch
    {
        _ when s.EndsWith("UL") => ulong.Parse(s),
        _ when s.LastOrDefault() == 'U' => uint.Parse(s),
        _ when s.LastOrDefault() == 'L' => long.Parse(s),
        _ when s.LastOrDefault() == 'F' => float.Parse(s),
        _ when s.LastOrDefault() == 'D' => double.Parse(s),
        _ when s.LastOrDefault() == 'M' => decimal.Parse(s),
        _ when s.Contains('.') => decimal.Parse(s),
        _ => long.Parse(s),
    };


    public static IStatement Parse(StatementSyntax syntax) => syntax switch
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

    public static Block Parse(BlockSyntax syntax) => new(CodeModelFactory.List(syntax.Statements.Select(Parse)));
    public static BreakStatement Parse(BreakStatementSyntax _) => new();
    public static CheckedStatement Parse(CheckedStatementSyntax syntax) => new(Parse(syntax.Block));
    public static ForEachStatement Parse(CommonForEachStatementSyntax syntax) => syntax switch
    {
        ForEachStatementSyntax statement => Parse(statement),
        _ => throw new ArgumentException($"Can't parse {nameof(ForEachStatement)} from '{syntax}'.")
    };
    public static ForEachStatement Parse(ForEachStatementSyntax syntax) => new(ParseType(syntax.Type), syntax.Identifier.ToString(), ParseExpression(syntax.Expression), Parse(syntax.Statement));
    public static ContinueStatement Parse(ContinueStatementSyntax _) => new();
    public static EmptyStatement Parse(EmptyStatementSyntax _) => new();
    public static ExpressionStatement Parse(ExpressionStatementSyntax syntax) => new(ParseExpression(syntax.Expression));
    public static FixedStatement Parse(FixedStatementSyntax syntax) => new(Parse(syntax.Declaration), Parse(syntax.Statement));
    public static VariableDeclarations Parse(VariableDeclarationSyntax syntax) => new(ParseType(syntax.Type), Parse(syntax.Variables));
    public static VariableDeclarator Parse(VariableDeclaratorSyntax syntax) => new(syntax.Identifier.ToString(), syntax.Initializer is null ? null : ParseExpression(syntax.Initializer.Value));
    public static List<VariableDeclarator> Parse(IEnumerable<VariableDeclaratorSyntax> syntax) => syntax.Select(Parse).ToList();
    public static List<Property> Parse(BracketedArgumentListSyntax syntax) => syntax.Arguments.Select(Parse).ToList();
    public static Property Parse(ArgumentSyntax syntax) => new(TypeShorthands.VoidType, syntax.NameColon?.ToString(), ParseExpression(syntax.Expression));  // TODO: Semantics for type
    public static List<Property> Parse(IEnumerable<ArgumentSyntax> syntax) => syntax.Select(Parse).ToList();
    public static AttributeList Parse(AttributeListSyntax syntax) => new(syntax.Target is null ? null : Parse(syntax.Target), syntax.Attributes.Select(Parse).ToList());
    public static AttributeTargetSpecifier Parse(AttributeTargetSpecifierSyntax syntax) => new(syntax.Identifier.ToString());
    public static Attribute Parse(AttributeSyntax syntax)
        => new(syntax.Name.ToString(), new(syntax.ArgumentList is null ? new List<AttributeArgument>() : syntax.ArgumentList.Arguments.Select(Parse).ToList()));
    public static AttributeArgument Parse(AttributeArgumentSyntax syntax) => new(ParseExpression(syntax.Expression), syntax.NameColon?.Name.ToString());
    public static ForStatement Parse(ForStatementSyntax syntax)
        => new(syntax.Declaration is null ? new(null) : Parse(syntax.Declaration), syntax.Initializers.Select(x => ParseExpression(x)).ToList(), ParseExpression(syntax.Condition), CodeModelFactory.List(syntax.Incrementors.Select(x => ParseExpression(x))), Parse(syntax.Statement));
    public static GotoStatement Parse(GotoStatementSyntax syntax) => new(ParseExpression(syntax.Expression));
    public static IfStatement Parse(IfStatementSyntax syntax) => new(ParseExpression(syntax.Condition), Parse(syntax.Statement), syntax.Else is null ? null : Parse(syntax.Else));
    public static IStatement Parse(ElseClauseSyntax syntax) => Parse(syntax.Statement);
    public static LabeledStatement Parse(LabeledStatementSyntax syntax) => new(syntax.Identifier.ToString(), Parse(syntax.Statement));
    public static LocalDeclarationStatements Parse(LocalDeclarationStatementSyntax syntax) => new(Parse(syntax.Declaration));
    public static LocalFunctionStatement Parse(LocalFunctionStatementSyntax syntax)
        => new(ParseModifier(syntax.Modifiers), ParseType(syntax.ReturnType), syntax.Identifier.ToString(),
            ParseTypes(syntax.TypeParameterList),
            ParseProperties(syntax.ParameterList), Parse(syntax.ConstraintClauses), syntax.Body is null ? null : Parse(syntax.Body),
            syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody.Expression));
    public static PropertyCollection ParseProperties(ParameterListSyntax syntax) => new(syntax.Parameters.Select(Parse));
    public static List<TypeParameterConstraintClause> Parse(IEnumerable<TypeParameterConstraintClauseSyntax> syntax) => syntax.Select(Parse).ToList();
    public static TypeParameterConstraintClause Parse(TypeParameterConstraintClauseSyntax syntax)
        => new(syntax.Name.ToString(), syntax.Constraints.Select(Parse).ToList());
    public static ITypeParameterConstraint Parse(TypeParameterConstraintSyntax syntax) => syntax switch
    {
        ClassOrStructConstraintSyntax constraint => Parse(constraint),
        ConstructorConstraintSyntax constraint => Parse(constraint),
        DefaultConstraintSyntax constraint => Parse(constraint),
        TypeConstraintSyntax constraint => Parse(constraint),
        _ => throw new ArgumentException($"Can't parse {nameof(ITypeParameterConstraint)} from '{syntax}'.")
    };
    public static ClassOrStructConstraint Parse(ClassOrStructConstraintSyntax syntax) => new(syntax.Kind(), syntax.ClassOrStructKeyword);
    public static ConstructorConstraint Parse(ConstructorConstraintSyntax _) => new();
    public static DefaultConstraint Parse(DefaultConstraintSyntax _) => new();
    public static TypeConstraint Parse(TypeConstraintSyntax syntax) => new(ParseType(syntax.Type));
    public static TypeCollection ParseTypes(TypeParameterListSyntax? syntax) => syntax is null ? new() : new(syntax.Parameters.Select(Parse));
    public static Property Parse(ParameterSyntax syntax) => new(syntax);
    public static IType Parse(TypeParameterSyntax syntax) => new QuickType(syntax.Identifier.ToString());    // TODO
    public static LockStatement Parse(LockStatementSyntax syntax) => new(ParseExpression(syntax.Expression), Parse(syntax.Statement));
    public static ReturnStatement Parse(ReturnStatementSyntax syntax) => new(ParseExpression(syntax.Expression));
    public static SwitchStatement Parse(SwitchStatementSyntax syntax) => new(ParseExpression(syntax.Expression), CodeModelFactory.List(syntax.Sections.Select(Parse)));
    public static SwitchSection Parse(SwitchSectionSyntax syntax) => throw new NotImplementedException();// new(CodeModelFactory.List(syntax.Labels.Select(ParseExpression)), Parse(syntax.Statements));
    //public static SwitchSection Parse(SwitchLabelSyntax syntax) => new(syntax.E);
    public static ThrowStatement Parse(ThrowStatementSyntax syntax) => new(ParseExpression(syntax.Expression));
    public static TryStatement Parse(TryStatementSyntax syntax) => new(Parse(syntax.Block), CodeModelFactory.List(syntax.Catches.Select(Parse)), syntax.Finally is null ? null : Parse(syntax.Finally));
    public static CatchClause Parse(CatchClauseSyntax syntax) => new(syntax.Declaration is null ? TypeShorthands.VoidType : ParseType(syntax.Declaration.Type), syntax.Declaration?.Identifier.ToString(), Parse(syntax.Block));
    public static CatchDeclaration Parse(CatchDeclarationSyntax syntax) => new(ParseType(syntax.Type), syntax.Identifier.ToString());
    public static FinallyClause Parse(FinallyClauseSyntax syntax) => new(Parse(syntax.Block));
    public static UnsafeStatement Parse(UnsafeStatementSyntax syntax) => new(Parse(syntax.Block));
    public static UsingStatement Parse(UsingStatementSyntax syntax) => new(Parse(syntax.Statement));
    public static WhileStatement Parse(WhileStatementSyntax syntax) => new(ParseExpression(syntax.Condition), Parse(syntax.Statement));

    public static Method Parse(MethodDeclarationSyntax syntax)
        => new(syntax.GetName(), new PropertyCollection(syntax), ParseType(syntax.ReturnType), syntax.Body is null ? null : Parse(syntax.Body), syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody.Expression));

    public static Constructor Parse(ConstructorDeclarationSyntax syntax)
        => new(syntax.Identifier.ToString(), new PropertyCollection(syntax), syntax.Body is null ? null : Parse(syntax.Body), syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody.Expression));
}
