using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeModels.AbstractCodeModels;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Extensions;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Access;
using CodeModels.Models.Primitives.Expression.CompileTime;
using CodeModels.Models.Primitives.Expression.Instantiation;
using CodeModels.Models.Primitives.Expression.Invocation;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Parsing;
using CodeModels.Reflection;
using Common.Extensions;
using Common.Reflection;
using Generator.Models.Primitives.Expression.AnonymousFunction;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Factory;

public static class AbstractCodeModelParsing
{
    public static AbstractProperty ParseProperty(ArgumentSyntax syntax, IType? specifiedType = null, SemanticModel? model = null) => syntax.Expression switch
    {
        TypeSyntax type => new(CodeModelParsing.Parse(type, model: model), syntax.NameColon?.Name.ToString()),
        DeclarationExpressionSyntax declaration => ParseProperty(declaration, specifiedType, model: model),
        ExpressionSyntax expression => new(CodeModelParsing.ParseExpression(expression, model: model)),
        _ => throw new ArgumentException($"Can't parse {nameof(AbstractProperty)} from '{syntax}'.")
    };

    public static AbstractProperty ParseProperty(DeclarationExpressionSyntax syntax, IType? type = null, SemanticModel? model = null)
        => new(CodeModelParsing.Parse(syntax.Type, model: model), syntax.Designation switch
        {
            null => default,
            SingleVariableDesignationSyntax designation => designation.Identifier.ToString(),
            _ => throw new ArgumentException($"Can't parse {nameof(AbstractProperty)} from '{syntax}'.")
        });

    public static NamedValueCollection ParseProperties(ParameterListSyntax syntax, SemanticModel? model = null) => new(syntax.Parameters.Select(x => CodeModelParsing.Parse(x, model)));

    public static NamedValueCollection ParseNamedValues(IType Type, IEnumerable<ExpressionSyntax> syntax, bool nameByIndex = false, SemanticModel? model = null)
       => new(syntax.Select((x, i) => new AbstractProperty(Type, nameByIndex ? $"Item{i + 1}" : null, CodeModelParsing.ParseExpression(x, Type, model))), specifiedType: Type);

    public static NamedValueCollection ParseNamedValues(IEnumerable<ArgumentSyntax> arguments, bool nameByIndex = false, IType? type = null, SemanticModel? model = null)
        => new(arguments.Select((x, i) => nameByIndex ? x.NameColon is null ? x.WithNameColon(NameColon($"Item{i + 1}")) : x : x).Select(x => ParseProperty(x, type, model)), specifiedType: type);

    public static NamedValueCollection ParseNamedValues(ArgumentListSyntax syntax, IType? type = null, SemanticModel? model = null) => ParseNamedValues(syntax.Arguments, type: type, model: model);

    public static NamedValueCollection ParseNamedValues(string code) => code.Parse(code).Members.FirstOrDefault() switch
    {
        ClassDeclarationSyntax declaration => new(declaration),
        RecordDeclarationSyntax declaration => new(declaration),
        GlobalStatementSyntax statement => ParseNamedValues(statement),
        _ => throw new ArgumentException($"Can't parse {nameof(NamedValueCollection)} from '{code}'.")
    };

    public static NamedValueCollection ParseNamedValues(GlobalStatementSyntax syntax) => syntax.Statement switch
    {
        ExpressionStatementSyntax expression => ParseNamedValues(expression.Expression),
        _ => throw new ArgumentException($"Can't parse {nameof(NamedValueCollection)} from '{syntax}'.")
    };

    public static NamedValueCollection ParseNamedValues(ExpressionSyntax syntax, IType? type = null) => syntax switch
    {
        TupleExpressionSyntax declaration => ParseNamedValues(declaration.Arguments, nameByIndex: true),
        TupleTypeSyntax declaration => new NamedValueCollection(declaration),
        _ => throw new ArgumentException($"Can't parse {nameof(NamedValueCollection)} from '{syntax}'.")
    };

    public static NamedValueCollection Parse(TupleExpressionSyntax syntax, IType? type = null, SemanticModel? model = null) => ParseNamedValues(syntax.Arguments, nameByIndex: true, model: model);


    public static AbstractProperty AbstractProperty(PropertyDeclarationSyntax property, Modifier modifier = Modifier.None, IType? interfaceType = null)
        => new(Type(property.Type), property.Identifier.ToString(), Expression(property.Initializer?.Value, Type(property.Type)), CodeModelParsing.Modifiers(property.Modifiers).SetModifiers(modifier), interfaceType: interfaceType);
    public static AbstractProperty AbstractProperty(TupleElementSyntax element, Modifier? modifier = null, IType? interfaceType = null)
        => new(Type(element.Type), element.Identifier.ToString(), modifier: modifier, interfaceType: interfaceType);
    public static AbstractProperty AbstractProperty(ParameterSyntax parameter, Modifier? modifier = null, IType? interfaceType = null)
        => new(Type(parameter.Type), parameter.Identifier.ToString(), Expression(parameter.Default?.Value, Type(parameter.Type)), modifier, interfaceType: interfaceType);
    public static AbstractProperty AbstractProperty(ITypeSymbol typeSymbol, string name, ExpressionSyntax? expression = null, Modifier? modifier = null, IType? interfaceType = null)
        => new(new TypeFromSymbol(typeSymbol), name, Expression(expression, new TypeFromSymbol(typeSymbol)), modifier, interfaceType: interfaceType);
    public static AbstractProperty AbstractProperty(TypeSyntax type, string name, ExpressionSyntax? expression = null, Modifier? modifier = null, TypeSyntax? interfaceType = null)
        => new(Type(type), name, Expression(expression, Type(type)), modifier, interfaceType: Type(interfaceType));
    public static AbstractProperty AbstractProperty(ITypeSymbol typeSymbol, string name, string? value = null, Modifier? modifier = null, IType? interfaceType = null)
        => new(new TypeFromSymbol(typeSymbol), name, value is null ? null : CodeModelFactory.Literal(value), modifier, interfaceType: interfaceType);


    //public static NamedValueCollection ParseNamedValues(IType Type, IEnumerable<ExpressionSyntax> syntax, bool nameByIndex = false, SemanticModel? model = null)
    //    => new(syntax.Select((x, i) =>  AbstractProperty(Type, nameByIndex ? $"Item{i + 1}" : null, ParseExpression(x, Type, model))), specifiedType: Type);

    //public static NamedValueCollection ParseNamedValues(IEnumerable<ArgumentSyntax> arguments, bool nameByIndex = false, IType? type = null, SemanticModel? model = null)
    //    => new(arguments.Select((x, i) => nameByIndex ? x.NameColon is null ? x.WithNameColon(NameColon($"Item{i + 1}")) : x : x).Select(x => ParseProperty(x, type, model)), specifiedType: type);

    //public static NamedValueCollection ParseNamedValues(ArgumentListSyntax syntax, IType? type = null, SemanticModel? model = null) => ParseNamedValues(syntax.Arguments, type: type, model: model);

}
