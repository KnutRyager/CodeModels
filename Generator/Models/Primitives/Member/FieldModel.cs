using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeAnalyzation.Models.CodeModelFactory;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzation.Models;

public record FieldModel(string Name,
    IType Type,
    List<AttributeList> Attributes,
    Modifier Modifier,
    IExpression Value)
    : FieldOrProperty<FieldDeclarationSyntax>(Name, Type, Attributes, Modifier, Value)
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

    public override IExpression AccessValue(IExpression? instance = null) => new FieldModelExpression(this, instance);

    public MemberAccessExpression Invoke(IExpression caller) => MemberAccess(this, caller);
    public MemberAccessExpression Invoke(string identifier) => Invoke(CodeModelFactory.Identifier(identifier));
    public MemberAccessExpression Invoke(string identifier, IType? type, ISymbol? symbol) => Invoke(Identifier(identifier, type, symbol));
    public MemberAccessExpression Invoke(string identifier, IType type) => Invoke(Identifier(identifier, type));
    public MemberAccessExpression Invoke(string identifier, ISymbol symbol) => Invoke(Identifier(identifier, symbol: symbol));


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

    public virtual IExpression EvaluateAccess(IProgramModelExecutionContext context, IExpression expression)
    {
        //var instance = expression is ThisExpression thisExpression ?
        //    thisExpression.Evaluate(context) as InstantiatedObject : expression is InstantiatedObject i ? i : null;
        if (expression is InstantiatedObject instance)
        {
            instance.EnterScopes(context);
        }
        else if (Owner is ClassModel2 baseType)
        {
            context.EnterScope(baseType.GetStaticScope());
        }
        try
        {
            //if (Value != VoidValue) return Value;
            return context.GetValue(Name);
        }
        finally
        {
            if (expression is InstantiatedObject instanceExit)
            {
                instanceExit.ExitScopes(context);
            }
            else if (Owner is ClassModel2)
            {
                context.ExitScope();
            }
        }
    }

    public virtual void Assign(IExpression value, IProgramModelExecutionContext context)
    {
        throw new NotImplementedException();
    }

    public FieldModelExpression Access(IExpression? instance = null) => new(this, instance);
    public AssignmentExpression Assign(IExpression value) => ToIdentifierExpression().Assign(value);
    public AssignmentExpression Assign(IExpression? caller, IExpression value) => Assignment(
        MemberAccess(caller ?? Owner?.ToIdentifierExpression() ?? throw new NotImplementedException(),
            ToIdentifierExpression()), value);
}

