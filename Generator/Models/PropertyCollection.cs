using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

public class PropertyCollection
{
    public List<Property> Properties { get; set; }

    public PropertyCollection(IEnumerable<Property> properties)
    {
        Properties = properties.ToList();
    }

    public PropertyCollection(ClassDeclarationSyntax declaration) : this(new PropertyVisiter().GetValues(declaration.SyntaxTree).Select(x => new Property(x))) { }
    public PropertyCollection(RecordDeclarationSyntax declaration) : this(new ParameterVisiter().GetValues(declaration.SyntaxTree).Select(x => new Property(x))) { }
    public PropertyCollection(TupleTypeSyntax declaration) : this(new TupleElementVisiter().GetValues(declaration.SyntaxTree).Select(x => new Property(x))) { }

    public static PropertyCollection Parse(string code) => code.Parse().Members.FirstOrDefault() switch
    {
        ClassDeclarationSyntax declaration => new PropertyCollection(declaration),
        RecordDeclarationSyntax declaration => new PropertyCollection(declaration),
        GlobalStatementSyntax statement => Parse(statement),
        _ => throw new ArgumentException($"Can't parse {nameof(PropertyCollection)} from '{code}'.")
    };

    public static PropertyCollection Parse(GlobalStatementSyntax statement) => statement.Statement switch
    {
        ExpressionStatementSyntax expression => Parse(expression.Expression),
        _ => throw new ArgumentException($"Can't parse {nameof(PropertyCollection)} from '{statement}'.")
    };

    public static PropertyCollection Parse(ExpressionSyntax expression) => expression switch
    {
        TupleExpressionSyntax declaration => Parse(declaration.Arguments),
        TupleTypeSyntax declaration => new PropertyCollection(declaration),
        _ => throw new ArgumentException($"Can't parse {nameof(PropertyCollection)} from '{expression}'.")
    };

    public static PropertyCollection Parse(IEnumerable<ArgumentSyntax> arguments)
        => new PropertyCollection(arguments.Select(x => Property.Parse(x)));


    public ClassDeclarationSyntax ToClass(string name) => ClassDeclaration(
            attributeLists: default,
            modifiers: TokenList(Token(SyntaxKind.PublicKeyword)),
            keyword: Token(SyntaxKind.ClassKeyword),
            identifier: Identifier(name),
            typeParameterList: default,
            baseList: default,
            constraintClauses: default,
            members: List<MemberDeclarationSyntax>(Properties.Select(x => x.ToProperty())),
            openBraceToken: Token(SyntaxKind.OpenBraceToken),
            closeBraceToken: Token(SyntaxKind.CloseBraceToken),
            semicolonToken: default
        );

    public RecordDeclarationSyntax ToRecord(string name, bool hasContent = false) => RecordDeclaration(
            attributeLists: default,
            modifiers: TokenList(Token(SyntaxKind.PublicKeyword)),
            keyword: Token(SyntaxKind.RecordKeyword),
            identifier: Identifier(name),
            typeParameterList: default,
            parameterList: ParameterList(SeparatedList(Properties.Select(x => x.ToParameter()))),
            baseList: default,
            constraintClauses: default,
            members: default,
            openBraceToken: hasContent ? Token(SyntaxKind.OpenBraceToken) : default,
            closeBraceToken: hasContent ? Token(SyntaxKind.CloseBraceToken) : default,
            semicolonToken: Token(SyntaxKind.SemicolonToken)
        );

    public TupleTypeSyntax ToTuple() => TupleType(
            elements: SeparatedList(Properties.Select(x => x.ToTupleElement()))
        );

}