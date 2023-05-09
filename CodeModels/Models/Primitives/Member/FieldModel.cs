using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeModels.Factory.CodeModelFactory;
using Microsoft.CodeAnalysis.CSharp;
using CodeModels.Factory;
using CodeModels.Execution;

namespace CodeModels.Models;

public record FieldModel(string Name,
    IType Type,
    List<AttributeList> Attributes,
    Modifier Modifier,
    IExpression Value)
    : FieldOrProperty<FieldDeclarationSyntax>(Name, Type, Attributes, Modifier, Value),
    IFieldModel
{
    public static FieldModel Create(string name,
    IType type,
    IEnumerable<AttributeList>? attributes = null,
    Modifier modifier = Modifier.Public,
    IExpression? value = null) => new(name,
    type,
    List(attributes),
    modifier,
    value ?? VoidValue);

    public override IInvocation AccessValue(IExpression? instance = null) => new FieldModelExpression(this, instance, GetScopes(instance));

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
        foreach (var attribute in Attributes) yield return attribute;
    }

    public override FieldDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
        => FieldDeclaration(
            SyntaxFactory.List(Attributes.Select(x => x.Syntax())),
            Modifier.Syntax(),
            VariableDeclaration().Syntax());

    private VariableDeclaration VariableDeclaration() => new(Type, Name, Value);

    public override IExpression EvaluateAccess(IExpression expression, IProgramModelExecutionContext context)
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

    public virtual void Assign(IExpression value, IProgramModelExecutionContext context, IList<IProgramModelExecutionScope> scopes)
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

    public FieldModelExpression Access(IExpression? instance = null) => new(this, instance, GetScopes());
    public AssignmentExpression Assign(IExpression value) => ToIdentifierExpression().Assign(value);
    public AssignmentExpression Assign(IExpression? caller, IExpression value) => Assignment(
        MemberAccess(caller ?? Owner?.ToIdentifierExpression() ?? throw new NotImplementedException(),
            ToIdentifierExpression()), value);

    public override void Assign(IExpression instance, IExpression value, IProgramModelExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

