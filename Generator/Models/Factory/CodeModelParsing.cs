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
    public static IExpression ParseExpression(ExpressionSyntax? syntax) => syntax switch
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
    public static ForEachStatement Parse(ForEachStatementSyntax syntax) => new(Parse(syntax.Type), syntax.Identifier.ToString(), ParseExpression(syntax.Expression), Parse(syntax.Statement));
    public static ContinueStatement Parse(ContinueStatementSyntax _) => new();
    public static EmptyStatement Parse(EmptyStatementSyntax _) => new();
    public static ExpressionStatement Parse(ExpressionStatementSyntax syntax) => new(ParseExpression(syntax.Expression));
    public static FixedStatement Parse(FixedStatementSyntax syntax) => new(Parse(syntax.Declaration), Parse(syntax.Statement));
    public static VariableDeclarations Parse(VariableDeclarationSyntax syntax) => new(Parse(syntax.Type), Parse(syntax.Variables));
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
        => new(syntax.Declaration is null ? new(null) : Parse(syntax.Declaration), syntax.Initializers.Select(ParseExpression).ToList(), ParseExpression(syntax.Condition), CodeModelFactory.List(syntax.Incrementors.Select(ParseExpression)), Parse(syntax.Statement));
    public static GotoStatement Parse(GotoStatementSyntax syntax) => new(ParseExpression(syntax.Expression));
    public static IfStatement Parse(IfStatementSyntax syntax) => new(ParseExpression(syntax.Condition), Parse(syntax.Statement), syntax.Else is null ? null : Parse(syntax.Else));
    public static IStatement Parse(ElseClauseSyntax syntax) => Parse(syntax.Statement);
    public static LabeledStatement Parse(LabeledStatementSyntax syntax) => new(syntax.Identifier.ToString(), Parse(syntax.Statement));
    public static LocalDeclarationStatements Parse(LocalDeclarationStatementSyntax syntax) => new(Parse(syntax.Declaration));
    public static LocalFunctionStatement Parse(LocalFunctionStatementSyntax syntax)
        => new(ParseModifier(syntax.Modifiers), Parse(syntax.ReturnType), syntax.Identifier.ToString(),
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
    public static TypeConstraint Parse(TypeConstraintSyntax syntax) => new(Parse(syntax.Type));
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
    public static CatchClause Parse(CatchClauseSyntax syntax) => new(syntax.Declaration is null ? TypeShorthands.VoidType : Parse(syntax.Declaration.Type), syntax.Declaration?.Identifier.ToString(), Parse(syntax.Block));
    public static CatchDeclaration Parse(CatchDeclarationSyntax syntax) => new(Parse(syntax.Type), syntax.Identifier.ToString());
    public static FinallyClause Parse(FinallyClauseSyntax syntax) => new(Parse(syntax.Block));
    public static UnsafeStatement Parse(UnsafeStatementSyntax syntax) => new(Parse(syntax.Block));
    public static UsingStatement Parse(UsingStatementSyntax syntax) => new(Parse(syntax.Statement));
    public static WhileStatement Parse(WhileStatementSyntax syntax) => new(ParseExpression(syntax.Condition), Parse(syntax.Statement));

    public static Method Parse(MethodDeclarationSyntax syntax)
        => new(syntax.GetName(), new PropertyCollection(syntax), Parse(syntax.ReturnType), syntax.Body is null ? null : Parse(syntax.Body), syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody.Expression));

    public static Constructor Parse(ConstructorDeclarationSyntax syntax)
        => new(syntax.Identifier.ToString(), new PropertyCollection(syntax), syntax.Body is null ? null : Parse(syntax.Body), syntax.ExpressionBody is null ? null : ParseExpression(syntax.ExpressionBody.Expression));
}
