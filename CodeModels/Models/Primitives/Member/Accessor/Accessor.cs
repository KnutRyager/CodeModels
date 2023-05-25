using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member;

public record Accessor(AccessorType Type,
    Block? Body,
    IExpression? ExpressionBody,
    List<AttributeList> Attributes,
    Modifier Modifier)
    : CodeModel<AccessorDeclarationSyntax>()
{
    public static Accessor Create(AccessorType type,
        Block? body = null,
        IExpression? expressionBody = null,
        IEnumerable<AttributeList>? attributes = null,
        Modifier modifier = Modifier.None)
        => new(type,
            body,
            expressionBody,
            CodeModelFactory.List(attributes),
            modifier);

    public override IEnumerable<ICodeModel> Children()
    {
        if (Body is not null) yield return Body;
        if (ExpressionBody is not null) yield return ExpressionBody;
        foreach (var attribute in Attributes) yield return attribute;
    }

    public override AccessorDeclarationSyntax Syntax()
        => SyntaxWithModifiers();

    public AccessorDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
        => Body is not null ? AccessorDeclaration(
            Type.ToSyntax(),
            List(Attributes.Select(x => x.Syntax())),
            modifier.SetModifiers(Modifier).SetFlags(removeModifier, false).Syntax(),
            Body.Syntax())
        : ExpressionBody is not null ? AccessorDeclaration(
            Type.ToSyntax(),
            List(Attributes.Select(x => x.Syntax())),
            modifier.SetModifiers(Modifier).SetFlags(removeModifier, false).Syntax(),
            Token(Type.ToKeyword()),
            ArrowExpressionClause(ExpressionBody.Syntax()),
            semicolonToken: Token(SyntaxKind.SemicolonToken))
            : AccessorDeclaration(
            Type.ToSyntax(),
            List(Attributes.Select(x => x.Syntax())),
            modifier.SetModifiers(Modifier).SetFlags(removeModifier, false).Syntax(),
            Token(Type.ToKeyword()),
            body: null!,
            semicolonToken: Token(SyntaxKind.SemicolonToken));

    public Method GetMethod(string name) => Type switch
    {
        AccessorType.Get => GetGetMethod(name),
        AccessorType.Set or AccessorType.Init => GetSetMethod(name),
        _ => throw new NotImplementedException()
    };

    private Method GetGetMethod(string name) => this switch
    {
        _ when Body is Block block => CodeModelFactory.Method(Type.GetBackingMethodName(name),
                       AbstractCodeModelFactory.NamedValues(),
                       CodeModelFactory.Type(typeof(void)), block),
        _ when ExpressionBody is IExpression expression => CodeModelFactory.Method(Type.GetBackingMethodName(name),
                       AbstractCodeModelFactory.NamedValues(),
                       CodeModelFactory.Type(typeof(void)), expression),
        _ => CodeModelFactory.Method(Type.GetBackingMethodName(name),
            AbstractCodeModelFactory.NamedValues(),
            CodeModelFactory.Type(typeof(void)), CodeModelFactory.IdentifierExp(Type.GetBackingFieldName(name)))
    };

    private Method GetSetMethod(string name) => this switch
    {
        _ when Body is Block block => CodeModelFactory.Method(Type.GetBackingMethodName(name),
                       AbstractCodeModelFactory.EmptyNamedValues("value"),
                       CodeModelFactory.Type(typeof(void)), block),
        _ when ExpressionBody is IExpression expression => CodeModelFactory.Method(Type.GetBackingMethodName(name),
                       AbstractCodeModelFactory.EmptyNamedValues("value"),
                       CodeModelFactory.Type(typeof(void)), expression),
        _ => CodeModelFactory.Method(Type.GetBackingMethodName(name),
            AbstractCodeModelFactory.NamedValues(AbstractCodeModelFactory.NamedValue("value")),
            CodeModelFactory.Type(typeof(void)), CodeModelFactory.Assignment(
                CodeModelFactory.IdentifierExp(Type.GetBackingFieldName(name)),
                CodeModelFactory.IdentifierExp("value")))
    };
}

public record GetAccessor(
    Block? Body,
    IExpression? ExpressionBody,
    List<AttributeList> Attributes,
    Modifier Modifier)
    : Accessor(AccessorType.Get, Body, ExpressionBody, Attributes, Modifier);

public record SetAccessor(
    Block? Body,
    IExpression? ExpressionBody,
    List<AttributeList> Attributes,
    Modifier Modifier)
    : Accessor(AccessorType.Set, Body, ExpressionBody, Attributes, Modifier);

public record InitAccessor(
    Block? Body,
    IExpression? ExpressionBody,
    List<AttributeList> Attributes,
    Modifier Modifier)
    : Accessor(AccessorType.Init, Body, ExpressionBody, Attributes, Modifier);

public record AddAccessor(
    Block? Body,
    IExpression? ExpressionBody,
    List<AttributeList> Attributes,
    Modifier Modifier)
    : Accessor(AccessorType.Add, Body, ExpressionBody, Attributes, Modifier);

public record RemoveAccessor(
    Block? Body,
    IExpression? ExpressionBody,
    List<AttributeList> Attributes,
    Modifier Modifier)
    : Accessor(AccessorType.Remove, Body, ExpressionBody, Attributes, Modifier);