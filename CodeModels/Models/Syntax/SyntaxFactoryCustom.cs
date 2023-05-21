using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CodeModels.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Generation;

public static class SyntaxFactoryCustom
{
    public static LiteralExpressionSyntax LiteralExpressionCustom(object? value) => value switch
    {
        short n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(n)),
        ushort n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(n)),
        int n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(n)),
        uint n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                n.ToString(NumberFormatInfo.InvariantInfo) + "U", n)),
        long n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                n.ToString(NumberFormatInfo.InvariantInfo) + "L", n)),
        ulong n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                n.ToString(NumberFormatInfo.InvariantInfo) + "UL", n)),
        float n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                ((float)n).ToString(NumberFormatInfo.InvariantInfo) + "F", (float)n)),
        double n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                ((double)n).ToString(NumberFormatInfo.InvariantInfo) + "D", (double)n)),
        decimal n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                n.ToString(NumberFormatInfo.InvariantInfo) + "M", n)),
        byte n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(n)),
        sbyte n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(n)),
        char n => LiteralExpression(SyntaxKind.CharacterLiteralExpression, Literal(n)),
        string s => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(s)),
        bool b => LiteralExpression(b ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression),
        null => LiteralExpression(SyntaxKind.NullLiteralExpression),
        _ => throw new ArgumentException($"Unhandled literal expression: '{value}'."),
    };

    public static ObjectCreationExpressionSyntax ConstructorCall(string name, IEnumerable<SyntaxNode> arguments)
        => ObjectCreationExpression(Token(ParseTrailingTrivia(""),
                SyntaxKind.NewKeyword, ParseTrailingTrivia(" ")),
                IdentifierName(name), ArgumentList(SeparatedList(arguments)), null);

    public static InvocationExpressionSyntax InvocationExpressionCustom(string name, IEnumerable<ExpressionSyntax> arguments)
        => InvocationExpressionCustom(IdentifierName(name), arguments);

    public static InvocationExpressionSyntax InvocationExpressionCustom(ExpressionSyntax expression, IEnumerable<ExpressionSyntax> arguments)
        => InvocationExpression(expression, ArgumentList(SeparatedList(arguments.Select(x => Argument(x)))));

    public static InvocationExpressionSyntax DottedInvocationExpressionCustom(string name, IEnumerable<ExpressionSyntax> arguments)
        => InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, arguments.First(), IdentifierName(name)),
            ArgumentList(SeparatedList(arguments?.Skip(1).Select(x => Argument(x)))));

    public static ClassDeclarationSyntax ClassDeclarationCustom(SyntaxList<AttributeListSyntax> attributeLists,
        SyntaxTokenList modifiers,
        SyntaxToken identifier,
        TypeParameterListSyntax? typeParameterList,
        BaseListSyntax? baseList,
        SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses,
        SyntaxList<MemberDeclarationSyntax> members) => ClassDeclaration(
            attributeLists: attributeLists,
            modifiers: modifiers,
            keyword: Token(SyntaxKind.ClassKeyword),
            identifier: identifier,
            typeParameterList: typeParameterList,
            baseList: baseList,
            constraintClauses: constraintClauses,
            openBraceToken: Token(SyntaxKind.OpenBraceToken),
            members: members,
            closeBraceToken: Token(SyntaxKind.CloseBraceToken),
            semicolonToken: default
        );

    public static InterfaceDeclarationSyntax InterfaceDeclarationCustom(SyntaxList<AttributeListSyntax> attributeLists,
        SyntaxTokenList modifiers,
        SyntaxToken identifier,
        TypeParameterListSyntax? typeParameterList,
        BaseListSyntax? baseList,
        SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses,
        SyntaxList<MemberDeclarationSyntax> members) => InterfaceDeclaration(
            attributeLists: attributeLists,
            modifiers: modifiers,
            keyword: Token(SyntaxKind.InterfaceKeyword),
            identifier: identifier,
            typeParameterList: typeParameterList,
            baseList: baseList,
            constraintClauses: constraintClauses,
            openBraceToken: Token(SyntaxKind.OpenBraceToken),
            members: members,
            closeBraceToken: Token(SyntaxKind.CloseBraceToken),
            semicolonToken: default
        );

    public static RecordDeclarationSyntax RecordDeclarationCustom(SyntaxList<AttributeListSyntax> attributeLists,
        SyntaxTokenList modifiers,
        SyntaxToken identifier,
        TypeParameterListSyntax? typeParameterList,
        ParameterListSyntax? parameterList,
        BaseListSyntax? baseList,
        SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses,
        SyntaxList<MemberDeclarationSyntax> members) => RecordDeclaration(
        attributeLists: attributeLists,
        modifiers: modifiers,
        keyword: Token(SyntaxKind.RecordKeyword),
        identifier: identifier,
        typeParameterList: typeParameterList,
        parameterList: parameterList,
        baseList: baseList,
        constraintClauses: constraintClauses,
        members: members,
        openBraceToken: RecordHasContent(members) ? Token(SyntaxKind.OpenBraceToken) : default,
        closeBraceToken: RecordHasContent(members) ? Token(SyntaxKind.CloseBraceToken) : default,
        semicolonToken: RecordHasContent(members) ? default : Token(SyntaxKind.SemicolonToken));

    public static StructDeclarationSyntax StructDeclarationCustom(SyntaxList<AttributeListSyntax> attributeLists,
        SyntaxTokenList modifiers,
        SyntaxToken identifier,
        TypeParameterListSyntax? typeParameterList,
        BaseListSyntax? baseList,
        SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses,
        SyntaxList<MemberDeclarationSyntax> members) => StructDeclaration(
        attributeLists: attributeLists,
        modifiers: modifiers,
        keyword: Token(SyntaxKind.StructKeyword),
        identifier: identifier,
        typeParameterList: typeParameterList,
        baseList: baseList,
        constraintClauses: constraintClauses,
        openBraceToken: Token(SyntaxKind.OpenBraceToken),
        members: members,
        closeBraceToken: Token(SyntaxKind.CloseBraceToken),
        semicolonToken: default);

    public static MethodDeclarationSyntax MethodDeclarationCustom(IEnumerable<AttributeListSyntax> attributeLists,
        SyntaxTokenList modifiers,
        TypeSyntax returnType,
        ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier,
        SyntaxToken identifier,
        TypeParameterListSyntax? typeParameterList,
        ParameterListSyntax? parameterList,
        BlockSyntax? body,
        ArrowExpressionClauseSyntax? expressionBody,
        SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => MethodDeclaration(
        attributeLists: List(attributeLists),
        modifiers: modifiers,
        returnType: returnType,
        explicitInterfaceSpecifier: explicitInterfaceSpecifier,
        identifier: identifier,
        typeParameterList: typeParameterList,
        parameterList: parameterList ?? ParameterList(),
        constraintClauses: constraintClauses,
        body: body,
        expressionBody: expressionBody,
        semicolonToken: SemicolonIfNone(body));

    public static LocalFunctionStatementSyntax LocalFunctionStatementCustom(IEnumerable<AttributeListSyntax> attributeLists,
        SyntaxTokenList modifiers,
        TypeSyntax returnType,
        SyntaxToken identifier,
        TypeParameterListSyntax? typeParameterList,
        ParameterListSyntax? parameterList,
        BlockSyntax? body,
        ArrowExpressionClauseSyntax? expressionBody,
        SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => LocalFunctionStatement(
        attributeLists: List(attributeLists),
        modifiers: modifiers,
        returnType: returnType,
        identifier: identifier,
        typeParameterList: typeParameterList,
        parameterList: parameterList ?? ParameterList(),
        constraintClauses: constraintClauses,
        body: body,
        expressionBody: expressionBody,
        semicolonToken: SemicolonIfNone(body));

    public static ParameterListSyntax ParameterListCustom(IEnumerable<ParameterSyntax> parameters) => ParameterList(Token(SyntaxKind.OpenParenToken),
         parameters: SeparatedList(parameters),
         closeParenToken: Token(SyntaxKind.CloseParenToken));

    public static ConstructorDeclarationSyntax ConstructorDeclarationCustom(SyntaxList<AttributeListSyntax> attributeLists,
         SyntaxTokenList modifiers,
         SyntaxToken identifier,
         ParameterListSyntax? parameterList,
         BlockSyntax? body,
         ConstructorInitializerSyntax? initializer = null,
         ArrowExpressionClauseSyntax? expressionBody = null) => ConstructorDeclaration(
         attributeLists: attributeLists,
         modifiers: modifiers,
         identifier: identifier,
         parameterList: parameterList ?? ParameterList(),
         initializer: initializer,
         body: body,
         expressionBody: expressionBody,
         semicolonToken: SemicolonIfNone(body));

    public static ArgumentListSyntax ArgumentListCustom(IEnumerable<ArgumentSyntax> arguments) => ArgumentList(
            arguments: SeparatedList(arguments));

    public static ArgumentSyntax ArgumentCustom(NameColonSyntax? nameColon,
        SyntaxToken refKindKeyword,
        ExpressionSyntax expression) => Argument(
            nameColon: nameColon,
            refKindKeyword: refKindKeyword,
            expression: expression);

    public static MemberDeclarationSyntax PropertyOrFieldDeclarationCustom(Modifier propertyType,
        SyntaxList<AttributeListSyntax> attributeLists,
        SyntaxTokenList modifiers,
        TypeSyntax type,
        ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier,
        SyntaxToken identifier,
        AccessorListSyntax accessorList,
        ArrowExpressionClauseSyntax? expressionBody,
        EqualsValueClauseSyntax? initializer) => propertyType.IsField() ? FieldDeclarationCustom(
            attributeLists: attributeLists,
            modifiers: modifiers,
            type: type,
            identifier: identifier,
            initializer: initializer)
        : PropertyDeclarationCustom(
            attributeLists: attributeLists,
            modifiers: modifiers,
            type: type,
            explicitInterfaceSpecifier: explicitInterfaceSpecifier,
            identifier: identifier,
            accessorList: accessorList,
            expressionBody: expressionBody,
            initializer: initializer);

    public static PropertyDeclarationSyntax PropertyDeclarationCustom(SyntaxList<AttributeListSyntax> attributeLists,
        SyntaxTokenList modifiers,
        TypeSyntax type,
        ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier,
        SyntaxToken identifier,
        AccessorListSyntax accessorList,
        ArrowExpressionClauseSyntax? expressionBody,
        EqualsValueClauseSyntax? initializer)
    {
        expressionBody ??= (IsGetOnly(accessorList) ? accessorList.Accessors[0].ExpressionBody : default);
        return PropertyDeclaration(
            attributeLists: attributeLists,
            modifiers: modifiers,
            type: type,
            explicitInterfaceSpecifier: explicitInterfaceSpecifier,
            identifier: identifier,
            accessorList: expressionBody == default ? accessorList : default,
            expressionBody: expressionBody,
            initializer: initializer,
            semicolonToken: initializer == default && expressionBody == default ? default : Token(SyntaxKind.SemicolonToken));
    }

    public static FieldDeclarationSyntax FieldDeclarationCustom(SyntaxList<AttributeListSyntax> attributeLists,
        SyntaxTokenList modifiers,
        TypeSyntax type,
        SyntaxToken identifier,
        EqualsValueClauseSyntax? initializer) => FieldDeclarationCustom(
            attributeLists: attributeLists,
            modifiers: modifiers,
            declaration: VariableDeclarationCustom(type, new[] { VariableDeclaratorCustom(identifier, default, initializer) }));

    public static IDictionary<string, string[]> Dependencies = new Dictionary<string, string[]>()
        {
            {"a", new string[]{ "b","c" } }
        };

    public static ObjectCreationExpressionSyntax ObjectCreationExpressionCustom(TypeSyntax type,
        ArgumentListSyntax? argumentList,
        InitializerExpressionSyntax? initializer) => ObjectCreationExpression(
            type: type,
            argumentList: argumentList,
            initializer: initializer);

    public static ObjectCreationExpressionSyntax DictionaryCreationExpressionCustom(TypeSyntax keyType,
        TypeSyntax valueType,
        ArgumentListSyntax? argumentList,
        InitializerExpressionSyntax? initializer) => ObjectCreationExpression(
            type: GenericName(Identifier("Dictionary"), TypeArgumentListCustom(keyType, valueType)),
            argumentList: argumentList ?? ArgumentList(),
            initializer: initializer);

    public static ObjectCreationExpressionSyntax ListInitializationCustom(TypeSyntax type,
        IEnumerable<ExpressionSyntax>? expressions = null) => ObjectCreationExpressionCustom(
            type: GenericName(Identifier("List"), TypeArgumentListCustom(type)),
            argumentList: default,
            initializer: expressions is null || !expressions.Any() ? default : CollectionInitializerExpressionCustom(expressions));

    public static ArrayCreationExpressionSyntax ArrayInitializationCustom(TypeSyntax type,
        IEnumerable<ExpressionSyntax>? expressions = null) => ArrayCreationExpressionCustom(ArrayType(type, rankSpecifiers: List(new[] { ArrayRankSpecifierCustom() })),
        initializer: expressions is null ? default : CollectionInitializerExpressionCustom(expressions));

    public static ArrayRankSpecifierSyntax ArrayRankSpecifierCustom(IEnumerable<ExpressionSyntax>? sizes = default)
        => ArrayRankSpecifier(sizes: sizes is null ? SeparatedList(new ExpressionSyntax[] { OmittedArraySizeExpression() }) : SeparatedList(sizes));

    public static TypeArgumentListSyntax TypeArgumentListCustom(params TypeSyntax[] arguments) => TypeArgumentList(arguments: SeparatedList(arguments));
    public static TypeArgumentListSyntax TypeArgumentListCustom(IEnumerable<TypeSyntax> arguments) => TypeArgumentList(arguments: SeparatedList(arguments));

    public static InitializerExpressionSyntax CollectionInitializerExpressionCustom(IEnumerable<ExpressionSyntax> expressions) => InitializerExpression(
            SyntaxKind.CollectionInitializerExpression,
            expressions: SeparatedList(expressions));

    public static InitializerExpressionSyntax ComplexElemementExpressionCustom(IEnumerable<ExpressionSyntax> expressions) => InitializerExpression(
            SyntaxKind.ComplexElementInitializerExpression,
            expressions: SeparatedList(expressions));

    public static ArrayCreationExpressionSyntax ArrayCreationExpressionCustom(ArrayTypeSyntax type, InitializerExpressionSyntax? initializer) => ArrayCreationExpression(
            type: type,
            initializer: initializer);

    public static FieldDeclarationSyntax FieldDeclarationCustom(SyntaxList<AttributeListSyntax> attributeLists,
        SyntaxTokenList modifiers,
        VariableDeclarationSyntax declaration) => FieldDeclaration(
            attributeLists: attributeLists,
            modifiers: modifiers,
            declaration: declaration,
            semicolonToken: Token(SyntaxKind.SemicolonToken));

    public static VariableDeclarationSyntax VariableDeclarationCustom(TypeSyntax type,
        IEnumerable<VariableDeclaratorSyntax> variables) => VariableDeclaration(
            type: type,
            variables: SeparatedList(variables));

    public static VariableDeclarationSyntax VariableDeclarationCustom(TypeSyntax type,
        VariableDeclaratorSyntax variable) => VariableDeclarationCustom(
            type: type,
            variables: new[] { variable });

    public static LocalDeclarationStatementSyntax LocalDeclarationStatementCustom(SyntaxTokenList modifiers,
        VariableDeclarationSyntax declaration) => LocalDeclarationStatement(
            modifiers: modifiers,
            declaration: declaration);

    public static VariableDeclaratorSyntax VariableDeclaratorCustom(SyntaxToken identifier,
         BracketedArgumentListSyntax? argumentList = default,
         EqualsValueClauseSyntax? initializer = default) => VariableDeclarator(
            identifier: identifier,
            argumentList: argumentList,
            initializer: initializer);

    public static VariableDeclaratorSyntax VariableDeclaratorCustom(SyntaxToken identifier,
         ExpressionSyntax? value) => VariableDeclaratorCustom(
            identifier: identifier,
            initializer: value is null ? null : EqualsValueClause(value));

    public static DeclarationExpressionSyntax DeclarationExpressionCustom(TypeSyntax type, VariableDesignationSyntax designation)
        => DeclarationExpression(type: type, designation: designation);

    public static DeclarationExpressionSyntax DeclarationExpressionCustom(TypeSyntax type, string identifier, ExpressionSyntax? value = null)
        => DeclarationExpression(type: type, designation: VariableDesignationCustom(identifier));

    public static VariableDesignationSyntax VariableDesignationCustom(string identifier)
        => SingleVariableDesignation(Identifier(identifier));

    public static AccessorListSyntax AccessorListCustom(IEnumerable<AccessorDeclarationSyntax> accessors) => AccessorList(
        accessors: List(accessors switch
        {
            _ => accessors
        }));

    public static AccessorDeclarationSyntax AccessorDeclarationGetCustom(SyntaxList<AttributeListSyntax> attributeLists,
        SyntaxTokenList modifiers,
         BlockSyntax? body,
         ArrowExpressionClauseSyntax? expressionBody) => AccessorDeclaration(SyntaxKind.GetAccessorDeclaration,
            attributeLists: attributeLists,
            modifiers: modifiers,
            keyword: Token(SyntaxKind.GetKeyword),
            body: body,
            expressionBody: expressionBody,
            semicolonToken: SemicolonIfNone(expressionBody));

    public static AccessorDeclarationSyntax AccessorDeclarationSetCustom(SyntaxList<AttributeListSyntax> attributeLists,
        SyntaxTokenList modifiers,
         BlockSyntax? body,
         ArrowExpressionClauseSyntax? expressionBody) => AccessorDeclaration(SyntaxKind.SetAccessorDeclaration,
            attributeLists: attributeLists,
            modifiers: modifiers,
            keyword: Token(SyntaxKind.SetKeyword),
            body: body,
            expressionBody: expressionBody,
            semicolonToken: SemicolonIfNone(expressionBody));

    public static ForStatementSyntax ForStatementCustom(
        VariableDeclarationSyntax? declaration,
        IEnumerable<ExpressionSyntax> initializers,
        ExpressionSyntax? condition,
        IEnumerable<ExpressionSyntax> incrementors,
        StatementSyntax statement) => ForStatement(declaration: declaration,
            initializers: SeparatedList(initializers),
            condition: condition,
            incrementors: SeparatedList(incrementors),
            statement: statement);

    public static ForEachStatementSyntax ForEachStatementCustom(
       TypeSyntax type,
       SyntaxToken identifier,
       ExpressionSyntax expression,
       StatementSyntax statement) => ForEachStatement(type, identifier, expression, statement);

    public static WhileStatementSyntax WhileStatementCustom(ExpressionSyntax condition, StatementSyntax statement)
        => WhileStatement(condition, statement);

    public static DoStatementSyntax DoStatementCustom(StatementSyntax statement, ExpressionSyntax condition)
        => DoStatement(statement, condition);

    public static ReturnStatementSyntax ReturnCustom(ExpressionSyntax expression) => ReturnStatement(expression);
    public static ContinueStatementSyntax ContinueCustom() => ContinueStatement();
    public static BreakStatementSyntax BreakCustom() => BreakStatement();

    public static IfStatementSyntax IfStatementCustom(
        ExpressionSyntax condition,
        StatementSyntax statement,
        ElseClauseSyntax? @else) => IfStatement(condition: condition,
            statement: statement,
            @else: @else);

    public static ElseClauseSyntax ElseClauseCustom(StatementSyntax statement) => ElseClause(statement: statement);

    public static SwitchStatementSyntax SwitchStatementCustom(ExpressionSyntax expression,
        IEnumerable<SwitchSectionSyntax> sections)
        => SwitchStatement(expression, List(sections));
    public static SwitchSectionSyntax SwitchSectionCustom(IEnumerable<SwitchLabelSyntax> labels, IEnumerable<StatementSyntax> statements)
        => SwitchSection(List(labels), List(statements));
    public static SwitchSectionSyntax SwitchSectionCustom(IEnumerable<SwitchLabelSyntax> labels, params StatementSyntax[] statements)
        => SwitchSection(List(labels), List(statements));
    public static SwitchSectionSyntax SwitchSectionCustom(SwitchLabelSyntax label, params StatementSyntax[] statements)
        => SwitchSection(List(new[] { label }), List(statements));
    public static CaseSwitchLabelSyntax SwitchLabelCustom(ExpressionSyntax expression) => CaseSwitchLabel(expression);
    public static DefaultSwitchLabelSyntax DefaultSwitchLabelCustom() => DefaultSwitchLabel();

    public static TryStatementSyntax TryStatementCustom(
       BlockSyntax block, IEnumerable<CatchClauseSyntax> catches, FinallyClauseSyntax? @finally)
        => TryStatement(List<AttributeListSyntax>(), block, List(catches), @finally);
    public static CatchClauseSyntax CatchClauseCustom(CatchDeclarationSyntax? declaration,
        BlockSyntax block,
        CatchFilterClauseSyntax? filter = null)
        => CatchClause(declaration, filter, block);
    public static CatchDeclarationSyntax CatchDeclarationCustom(TypeSyntax type, string? identifier = null)
        => identifier is null ? CatchDeclaration(type) : CatchDeclaration(type, Identifier(identifier));
    public static FinallyClauseSyntax FinallyClauseCustom(BlockSyntax block) => FinallyClause(block);

    public static ThrowStatementSyntax ThrowStatementCustom(ExpressionSyntax expression) => ThrowStatement(expression);
    public static ThrowExpressionSyntax ThrowExpressionCustom(ExpressionSyntax expression) => ThrowExpression(expression);
    public static SyntaxToken TupleNameIdentifier(string? name) => name == default || new Regex("Item+[1-9]+[0-9]*").IsMatch(name) ? default : SyntaxFactory.Identifier(name);

    private static bool RecordHasContent(SyntaxList<MemberDeclarationSyntax>? members) => members?.Any() ?? false;
    private static bool IsGetOnly(AccessorListSyntax accessorList) => accessorList.Accessors.Count == 1 && accessorList.Accessors[0].Keyword.IsKind(SyntaxKind.GetKeyword);

    private static SyntaxToken SemicolonIfNone(params object?[]? potentialContent) => potentialContent?.Any(x => x != default) == true ? default : Token(SyntaxKind.SemicolonToken);
}
