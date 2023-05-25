using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.AbstractCodeModels.Member;
using CodeModels.Collectors;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Factory;

public static class AbstractCodeModelParsing
{
    public static AbstractProperty ParseProperty(this CodeModelParser parser, ArgumentSyntax syntax, IType? specifiedType = null) => syntax.Expression switch
    {
        TypeSyntax type => new(parser.Parse(type), syntax.NameColon?.Name.ToString()),
        DeclarationExpressionSyntax declaration => parser.ParseProperty(declaration, specifiedType),
        ExpressionSyntax expression => new(parser.ParseExpression(expression)),
        _ => throw new ArgumentException($"Can't parse {nameof(AbstractProperty)} from '{syntax}'.")
    };

    public static AbstractProperty ParseProperty(this CodeModelParser parser, DeclarationExpressionSyntax syntax, IType? type = null)
        => new(parser.Parse(syntax.Type), syntax.Designation switch
        {
            null => default,
            SingleVariableDesignationSyntax designation => designation.Identifier.ToString(),
            _ => throw new ArgumentException($"Can't parse {nameof(AbstractProperty)} from '{syntax}'.")
        });

    public static NamedValueCollection ParseProperties(this CodeModelParser parser, ParameterListSyntax syntax) => new(syntax.Parameters.Select(x => parser.Parse(x)));

    public static NamedValueCollection ParseNamedValues(this CodeModelParser parser, IType Type, IEnumerable<ExpressionSyntax> syntax, bool nameByIndex = false)
       => new(syntax.Select((x, i) => new AbstractProperty(Type, nameByIndex ? $"Item{i + 1}" : null, parser.ParseExpression(x, Type))), specifiedType: Type);

    public static NamedValueCollection ParseNamedValues(this CodeModelParser parser, IEnumerable<ArgumentSyntax> arguments, bool nameByIndex = false, IType? type = null)
        => new(arguments.Select((x, i) => nameByIndex ? x.NameColon is null ? x.WithNameColon(SyntaxFactory.NameColon($"Item{i + 1}")) : x : x).Select(x => ParseProperty(parser, x, type)), specifiedType: type);

    public static NamedValueCollection ParseNamedValues(this CodeModelParser parser, ArgumentListSyntax syntax, IType? type = null) => ParseNamedValues(parser, syntax.Arguments, type: type);

    public static NamedValueCollection ParseNamedValues(string code)
    {
        var parser = new CodeModelParser(code);
        return parser.CompilationUnit.Members.FirstOrDefault() switch
        {
            ClassDeclarationSyntax declaration => NamedValues(parser, declaration),
            RecordDeclarationSyntax declaration => NamedValues(parser, declaration),
            GlobalStatementSyntax statement => ParseNamedValues(statement),
            _ => throw new ArgumentException($"Can't parse {nameof(NamedValueCollection)} from '{code}'.")
        };
    }

    public static NamedValueCollection ParseNamedValues(GlobalStatementSyntax syntax) => syntax.Statement switch
    {
        ExpressionStatementSyntax expression => ParseNamedValues(null!, expression.Expression),
        _ => throw new ArgumentException($"Can't parse {nameof(NamedValueCollection)} from '{syntax}'.")
    };

    public static NamedValueCollection ParseNamedValues(this CodeModelParser parser, ExpressionSyntax syntax, IType? type = null) => syntax switch
    {
        TupleExpressionSyntax declaration => ParseNamedValues(parser, declaration.Arguments, nameByIndex: true),
        TupleTypeSyntax declaration => NamedValues(parser, declaration),
        _ => throw new ArgumentException($"Can't parse {nameof(NamedValueCollection)} from '{syntax}'.")
    };

    public static NamedValueCollection NamedValues(this CodeModelParser parser, ClassDeclarationSyntax declaration) => new(new PropertyVisiter().GetEntries(declaration.SyntaxTree).Select(x => AbstractProperty(parser, x)), declaration.Identifier.ToString());
    public static NamedValueCollection NamedValues(this CodeModelParser parser, RecordDeclarationSyntax declaration) => new(new ParameterVisiter().GetEntries(declaration.SyntaxTree).Select(x => AbstractProperty(parser, x)), declaration.Identifier.ToString());
    public static NamedValueCollection NamedValues(this CodeModelParser parser, TupleTypeSyntax declaration) => new(new TupleElementVisiter().GetEntries(declaration.SyntaxTree).Select(x => AbstractProperty(parser, x)));
    public static NamedValueCollection NamedValues(this CodeModelParser parser, MethodDeclarationSyntax declaration) => NamedValues(parser, declaration.ParameterList);
    public static NamedValueCollection NamedValues(this CodeModelParser parser, ConstructorDeclarationSyntax declaration) => NamedValues(parser, declaration.ParameterList);
    public static NamedValueCollection NamedValues(this CodeModelParser parser, ParameterListSyntax parameters) => new(parameters.Parameters.Select(x => AbstractProperty(parser, x)));

    public static NamedValueCollection Parse(this CodeModelParser parser, TupleExpressionSyntax syntax, IType? type = null) => ParseNamedValues(parser, syntax.Arguments, nameByIndex: true);

    public static AbstractProperty AbstractProperty(this CodeModelParser parser, PropertyDeclarationSyntax property, Modifier modifier = Modifier.None, IType? interfaceType = null)
        => new(Type(property.Type), property.Identifier.ToString(), parser.ParseExpression(property.Initializer?.Value, Type(property.Type)), CodeModelParsing.Modifiers(property.Modifiers).SetModifiers(modifier), interfaceType: interfaceType);
    public static AbstractProperty AbstractProperty(this CodeModelParser parser, TupleElementSyntax element, Modifier? modifier = null, IType? interfaceType = null)
        => new(Type(element.Type), element.Identifier.ToString(), modifier: modifier, interfaceType: interfaceType);
    public static AbstractProperty AbstractProperty(this CodeModelParser parser, ParameterSyntax parameter, Modifier? modifier = null, IType? interfaceType = null)
        => new(Type(parameter.Type), parameter.Identifier.ToString(), parser.ParseExpression(parameter.Default?.Value, Type(parameter.Type)), modifier, interfaceType: interfaceType);
    public static AbstractProperty AbstractProperty(this CodeModelParser parser, ITypeSymbol typeSymbol, string name, ExpressionSyntax? expression = null, Modifier? modifier = null, IType? interfaceType = null)
        => new(new TypeFromSymbol(typeSymbol), name, parser.ParseExpression(expression, new TypeFromSymbol(typeSymbol)), modifier, interfaceType: interfaceType);
    public static AbstractProperty AbstractProperty(IType type, string name, IExpression? expression = null, Modifier? modifier = null, IType? interfaceType = null)
        => new(type, name, expression, modifier, interfaceType: interfaceType);
    public static AbstractProperty AbstractProperty(ITypeSymbol typeSymbol, string name, string? value = null, Modifier? modifier = null, IType? interfaceType = null)
        => new(new TypeFromSymbol(typeSymbol), name, value is null ? null : Literal(value), modifier, interfaceType: interfaceType);

    public static ExpressionsMap ExpressionsMap(IExpression key, EnumDeclarationSyntax declaration)
        => AbstractCodeModels.Collection.ExpressionsMap.Create(key, declaration);

    public static TypeCollection Types(IEnumerable<IType>? values = null) => TypeCollection.Create(values);
}
