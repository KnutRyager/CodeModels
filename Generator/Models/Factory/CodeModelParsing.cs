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
    public static IType ParseType(SyntaxToken token) => ParseType(token.ToString());
    public static IType ParseType(string code) => Parse(ParseTypeName(code));

    // TODO
    public static IType Parse(TypeSyntax? type, bool required = true, TypeSyntax? fullType = null) => type switch
    {
        PredefinedTypeSyntax t => new QuickType(t.Keyword.ToString(), required, SourceSyntax: fullType ?? t),
        NullableTypeSyntax t => Parse(t.ElementType, false, fullType: fullType ?? t),
        IdentifierNameSyntax t => new QuickType(t.Identifier.ToString(), SourceSyntax: fullType ?? t),
        ArrayTypeSyntax t => new QuickType(t.ElementType.ToString(), IsMulti: true, SourceSyntax: fullType ?? t),
        GenericNameSyntax t => new QuickType(t.Identifier.ToString(), SourceSyntax: fullType ?? t),
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

    public static Property ParseProperty(ArgumentSyntax syntax) => syntax.Expression switch
    {
        TypeSyntax type => new(Parse(type), syntax.NameColon?.Name.ToString()),
        DeclarationExpressionSyntax declaration => ParseProperty(declaration),
        _ => throw new ArgumentException($"Can't parse {nameof(Property)} from '{syntax}'.")
    };

    public static Property ParseProperty(DeclarationExpressionSyntax syntax) => new(Parse(syntax.Type), syntax.Designation switch
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

    public static PropertyCollection ParsePropertyCollection(ExpressionSyntax syntax) => syntax switch
    {
        TupleExpressionSyntax declaration => ParsePropertyCollection(declaration.Arguments, nameByIndex: true),
        TupleTypeSyntax declaration => new PropertyCollection(declaration),
        _ => throw new ArgumentException($"Can't parse {nameof(PropertyCollection)} from '{syntax}'.")
    };

    public static PropertyCollection ParsePropertyCollection(IEnumerable<ArgumentSyntax> arguments, bool nameByIndex = false)
        => new(arguments.Select((x, i) => nameByIndex ? x.NameColon is null ? x.WithNameColon(NameColon($"Item{i + 1}")) : x : x).Select(ParseProperty));

    public static IExpression ParseFromQualifiedName(string qualifiedName) => new ExpressionFromSyntax(qualifiedName);
    public static IExpression Parse(ExpressionSyntax? syntax) => syntax switch
    {
        null => CodeModelFactory.VoidValue,
        LiteralExpressionSyntax literal => literal.Kind() switch
        {
            SyntaxKind.NullLiteralExpression => CodeModelFactory.NullValue,
            _ => throw new ArgumentException($"Unhandled literal kind '{literal}'.")
        },
        _ => new ExpressionFromSyntax(syntax)
    };
    public static IStatement Parse(StatementSyntax syntax) => syntax switch
    {

        _ => throw new ArgumentException($"Can't parse {nameof(IStatement)} from '{syntax}'.")
    };
    public static Method Parse(MethodDeclarationSyntax syntax)
        => new(syntax.GetName(), new PropertyCollection(syntax), Parse(syntax.ReturnType), syntax.Body is null ? null : Parse(syntax.Body), syntax.ExpressionBody is null ? null : Parse(syntax.ExpressionBody.Expression));

    public static Constructor Parse(ConstructorDeclarationSyntax syntax)
        => new(syntax.Identifier.ToString(), new PropertyCollection(syntax), syntax.Body is null ? null : Parse(syntax.Body), syntax.ExpressionBody is null ? null : Parse(syntax.ExpressionBody.Expression));

    public static Block Parse(BlockSyntax syntax) => syntax switch
    {
        // TODO
        _ => throw new ArgumentException($"Can't parse {nameof(IStatement)} from '{syntax}'.")
    };
}
