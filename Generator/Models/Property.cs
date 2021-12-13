using System;
using System.Text.RegularExpressions;
using CodeAnalyzation.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

public class Property
{
    public string? Name { get; set; }
    public TType Type { get; set; }
    public string? DefaultValue { get; set; }
    public ExpressionSyntax? defaultValueExpression { get; set; }

    public Property(TType type, string? name, string? defaultValue = null, ExpressionSyntax? defaultExpression = null)
    {
        Type = type;
        Name = name;
        DefaultValue = defaultValue;
        defaultValueExpression = defaultExpression;
    }

    public Property(PropertyDeclarationSyntax property)
        : this(TType.Parse(property.Type), property.Identifier.ToString(), defaultExpression: property.Initializer?.Value) { }

    public Property(TupleElementSyntax element)
        : this(TType.Parse(element.Type), element.Identifier.ToString()) { }

    public Property(ParameterSyntax parameter)
        : this(TType.Parse(parameter.Type), parameter.Identifier.ToString(), defaultExpression: parameter.Default?.Value) { }

    public Property(ITypeSymbol typeSymbol, string name, string? defaultValue = null, ExpressionSyntax? defaultExpression = null)
        : this(new TType(typeSymbol), name, defaultValue, defaultExpression) { }

    public static Property Parse(ArgumentSyntax argument) => argument.Expression switch
    {
        TypeSyntax type => new(TType.Parse(type), default),
        DeclarationExpressionSyntax declaration => Parse(declaration),
        _ => throw new ArgumentException($"Can't parse {nameof(Property)} from '{argument}'.")
    };

    public static Property Parse(DeclarationExpressionSyntax declaration) => new(TType.Parse(declaration.Type), declaration.Designation switch
    {
        null => default,
        SingleVariableDesignationSyntax designation => designation.Identifier.ToString(),
        _ => throw new ArgumentException($"Can't parse {nameof(Property)} from '{declaration}'.")
    });

    public static Property DefaultExpression(TType type, string name, ExpressionSyntax defaultValue) => new(type, name, defaultExpression: defaultValue);
    public static Property DefaultExpression(TType type, string name, string defaultValue) => new(type, name, defaultValue: defaultValue);

    public ParameterSyntax ToParameter() => Parameter(
            attributeLists: default,
            modifiers: default,
            type: TypeSyntax(),
            identifier: Identifier(Name!),
            @default: Initializer());

    public PropertyDeclarationSyntax ToProperty() => PropertyDeclaration(
            attributeLists: default,
                    modifiers: new SyntaxTokenList(Token(SyntaxKind.PublicKeyword)),
            type: TypeSyntax(),
            explicitInterfaceSpecifier: default,
            identifier: Identifier(Name!),
            accessorList: AccessorList(new SyntaxList<AccessorDeclarationSyntax>(new[]{
                AccessorDeclaration(SyntaxKind.GetAccessorDeclaration,
                    attributeLists:default,
                    modifiers: default,
                    keyword: Token(SyntaxKind.GetKeyword),
                    body:default,
                    expressionBody:default,
                    semicolonToken: Token(SyntaxKind.SemicolonToken)),
                AccessorDeclaration(SyntaxKind.SetAccessorDeclaration,
                    attributeLists:default,
                    modifiers: default,
                    keyword: Token(SyntaxKind.SetKeyword),
                    body:default,
                    expressionBody:default,
                    semicolonToken: Token(SyntaxKind.SemicolonToken))

                    })),
            expressionBody: default,
            initializer: Initializer(),
            semicolonToken: Initializer() == default ? default : Token(SyntaxKind.SemicolonToken));

    public TupleElementSyntax ToTupleElement() => TupleElement(type: TypeSyntax(), identifier: TupleNameIdentifier(Name));

    public ExpressionSyntax? DefaultValueSyntax() => defaultValueExpression ?? DefaultValue switch
    {
        object _ => LiteralExpressionCustom(DefaultValue),
        _ => default
    };

    public TypeSyntax TypeSyntax() => Type.TypeSyntax();

    public EqualsValueClauseSyntax? Initializer() => DefaultValueSyntax() switch
    {
        ExpressionSyntax expression => EqualsValueClause(expression),
        _ => default
    };

    private SyntaxToken TupleNameIdentifier(string? name) => name == default || new Regex("Item+[1-9]+[0-9]*").IsMatch(name) ? default : Identifier(name);
}
