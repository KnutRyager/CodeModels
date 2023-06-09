﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeModels.Factory.CodeModelFactory;
using Microsoft.CodeAnalysis.CSharp;
using CodeModels.Factory;
using CodeModels.Execution.Scope;
using CodeModels.Execution.Context;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Access;
using CodeModels.Models.Primitives.Attribute;

namespace CodeModels.Models.Primitives.Member;

public record Field(string Name,
    IType Type,
    AttributeListList Attributes,
    Modifier Modifier,
    IExpression? Value)
    : FieldOrProperty<FieldDeclarationSyntax>(Name, Type, Attributes, Modifier, Value),
    IField<FieldExpression>
{
    public static Field Create(string name,
    IType type,
    IToAttributeListListConvertible? attributes = null,
    Modifier modifier = Modifier.Public,
    IExpression? value = null) => new(name,
    type,
    attributes?.ToAttributeListList() ?? AttributesList(),
    modifier,
    value ?? VoidValue);

    public override IInvocation AccessValue(IExpression? instance = null) => new FieldExpression(this, instance, GetScopes(instance));

    public MemberAccessExpression Invoke(IExpression caller) => MemberAccess(this, caller);
    public MemberAccessExpression Invoke(string identifier) => Invoke(CodeModelFactory.Identifier(identifier));
    public MemberAccessExpression Invoke(string identifier, IType? type, ISymbol? symbol) => Invoke(Identifier(identifier, type, symbol));
    public MemberAccessExpression Invoke(string identifier, IType type) => Invoke(Identifier(identifier, type));
    public MemberAccessExpression Invoke(string identifier, ISymbol symbol) => Invoke(Identifier(identifier, symbol: symbol));
    public override IInvocation Invoke(IExpression? caller, IEnumerable<IExpression> _)
        => AccessValue(caller);

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        foreach (var attribute in Attributes.Children()) yield return attribute;
    }

    public override FieldDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
        => FieldDeclaration(
            Attributes.Syntax(),
            Modifier.Syntax(),
            VariableDeclaration().Syntax());

    private VariableDeclaration VariableDeclaration() => new(Type, Name, Value);

    public override IExpression EvaluateAccess(IExpression? expression, ICodeModelExecutionContext context)
    {
        var scopes = GetScopes(expression);
        try
        {
            context.EnterScopes(scopes);
            //var instance = expression is ThisExpression thisExpression ?
            //    thisExpression.Evaluate(context) as InstantiatedObject : expression is InstantiatedObject i ? i : null;
            //if (expression is InstantiatedObject instance)
            //{
            //    instance.EnterScopes(context);
            //}
            //else if (Owner is ClassDeclaration baseType)
            //{
            //    context.EnterScope(baseType.GetStaticScope());
            //}
            //else if (expression is IdentifierExpression identifier && identifier.Model is ClassDeclaration staticReference)
            //{
            //    context.EnterScope(staticReference.GetStaticScope());
            //}
            //if (Value != VoidValue) return Value;
            return context.GetValue(Name);
        }
        finally
        {
            context.ExitScopes(scopes);
        }
    }

    public override void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes)
    {
        try
        {
            context.EnterScopes(scopes);
            Assign(value).Evaluate(context);
        }
        finally
        {
            context.ExitScopes(scopes);
        }
    }

    public FieldExpression Access(IExpression? instance = null) => new(this, instance, GetScopes());
    IFieldExpression IField.Access(IExpression? instance) => Access(instance);

    public AssignmentExpression Assign(IExpression value) => ToIdentifierExpression().Assign(value);
    public override AssignmentExpression Assign(IExpression? caller, IExpression value) => Assignment(
        MemberAccess(caller ?? Owner?.ToIdentifierExpression() ?? throw new NotImplementedException(),
            ToIdentifierExpression()), value);

    public override void Assign(IExpression? instance, IExpression value, ICodeModelExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

