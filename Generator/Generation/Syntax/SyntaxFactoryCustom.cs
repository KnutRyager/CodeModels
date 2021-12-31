using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Generation
{
    public static class SyntaxFactoryCustom
    {
        public static LiteralExpressionSyntax LiteralExpressionCustom(object value) => value switch
        {
            short n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(n)),
            ushort n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(n)),
            int n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(n)),
            uint n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                    n.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "U", n)),
            long n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                    n.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "L", n)),
            ulong n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                    n.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "UL", n)),
            float n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                    ((float)n).ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "F", (float)n)),
            double n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                    ((double)n).ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "D", (double)n)),
            decimal n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(
                    n.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "M", n)),
            byte n => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(n)),
            char n => LiteralExpression(SyntaxKind.CharacterLiteralExpression, Literal(n)),
            string s => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(s)),
            bool b => LiteralExpression(b ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression),
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

        public static MethodDeclarationSyntax MethodDeclarationCustom(SyntaxList<AttributeListSyntax> attributeLists,
            SyntaxTokenList modifiers,
            TypeSyntax returnType,
            ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier,
            SyntaxToken identifier,
            TypeParameterListSyntax? typeParameterList,
            ParameterListSyntax? parameterList,
            BlockSyntax? body,
            ArrowExpressionClauseSyntax? expressionBody,
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses) => MethodDeclaration(
            attributeLists: attributeLists,
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

        public static ParameterListSyntax ParameterListCustom(IEnumerable<ParameterSyntax> parameters) => ParameterList(Token(SyntaxKind.OpenParenToken),
             parameters: SeparatedList(parameters),
             closeParenToken: Token(SyntaxKind.CloseParenToken));

        public static ConstructorDeclarationSyntax ConstructorDeclarationCustom(SyntaxList<AttributeListSyntax> attributeLists,
             SyntaxTokenList modifiers,
             SyntaxToken identifier,
             ParameterListSyntax? parameterList,
             BlockSyntax? body,
             ConstructorInitializerSyntax? initializer,
             ArrowExpressionClauseSyntax? expressionBody) => ConstructorDeclaration(
             attributeLists: attributeLists,
             modifiers: modifiers,
             identifier: identifier,
             parameterList: parameterList ?? ParameterList(),
             initializer: initializer,
             body: body,
             expressionBody: expressionBody,
             semicolonToken: SemicolonIfNone(body));

        public static ArgumentListSyntax ArgumentListCustom(IEnumerable<ArgumentSyntax> arguments) => ArgumentList(
                openParenToken: Token(SyntaxKind.OpenBraceToken),
                arguments: SeparatedList(arguments),
                closeParenToken: Token(SyntaxKind.CloseBraceToken));

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

        public static VariableDeclaratorSyntax VariableDeclaratorCustom(SyntaxToken identifier,
             BracketedArgumentListSyntax? argumentList,
             EqualsValueClauseSyntax? initializer) => VariableDeclarator(
                identifier: identifier,
                argumentList: argumentList,
                initializer: initializer);

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

        private static bool RecordHasContent(SyntaxList<MemberDeclarationSyntax>? members) => members?.Any() ?? false;
        private static bool IsGetOnly(AccessorListSyntax accessorList) => accessorList.Accessors.Count == 1 && accessorList.Accessors[0].Keyword.IsKind(SyntaxKind.GetKeyword);

        private static SyntaxToken SemicolonIfNone(params object?[]? potentialContent) => potentialContent?.Any(x => x != default) == true ? default : Token(SyntaxKind.SemicolonToken);
    }
}