using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models;

public record InstantiatedObject(ClassDeclaration Type,
    IProgramModelExecutionScope Scope,
    IProgramModelExecutionScope StaticScope,
    IProgramModelExecutionScope? ParentScope = null) : IScopeHolder, IExpression
{
    public bool IsLiteralExpression => false;

    public LiteralExpressionSyntax? LiteralSyntax() => default;

    public object? LiteralValue() => this;

    public SimpleNameSyntax NameSyntax() => Type.NameSyntax();

    public ExpressionStatement AsStatement()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ICodeModel> Children()
    {
        throw new NotImplementedException();
    }

    public string Code()
    {
        throw new NotImplementedException();
    }

    public ISet<IType> Dependencies(ISet<IType>? set = null)
    {
        throw new NotImplementedException();
    }

    public IList<IProgramModelExecutionScope> GetScopes(IProgramModelExecutionContext context)
        => new[] { StaticScope, Scope };

    public void EnterScopes(IProgramModelExecutionContext context)
    {
        context.EnterScope(StaticScope);
        context.EnterScope(Scope);
    }

    public void ExitScopes(IProgramModelExecutionContext context)
    {
        context.ExitScope(Scope);
        context.ExitScope(StaticScope);
    }

    public IExpression Evaluate(IProgramModelExecutionContext context) => this;
    public object? EvaluatePlain(IProgramModelExecutionContext context) => this;

    public IExpression GetValue(string identifier) => Type.TryGetMember(identifier) switch
    {
        FieldModel _ => Scope.GetValue(identifier),
        //PropertyModel property => Literal(property.AccessValue(_object)),
        Method _ => throw new ProgramModelExecutionException($"Cannot get value of method '{identifier}'"),
        _ => throw new ProgramModelExecutionException($"Cannot get non-found identifier '{identifier}'")
    };

    public IType Get_Type() => Type.Get_Type();

    public IdentifierNameSyntax IdentifierNameSyntax()
    {
        throw new NotImplementedException();
    }

    public SyntaxToken IdentifierSyntax()
    {
        throw new NotImplementedException();
    }

    public ExpressionSyntax Syntax()
    {
        throw new NotImplementedException();
    }

    public ArgumentSyntax ToArgument()
    {
        throw new NotImplementedException();
    }

    public EnumMemberDeclarationSyntax ToEnumValue(int? value = null)
    {
        throw new NotImplementedException();
    }

    public IdentifierExpression ToIdentifierExpression()
    {
        throw new NotImplementedException();
    }

    ExpressionOrPatternSyntax IExpressionOrPattern.Syntax() => Syntax();
    CSharpSyntaxNode ICodeModel.Syntax() => Syntax();
}