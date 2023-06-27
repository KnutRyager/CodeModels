using System;
using System.Collections.Generic;
using CodeModels.Execution;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record InstantiatedObject(IClassOrRecordDeclaration Type,
    ICodeModelExecutionScope Scope,
    ICodeModelExecutionScope StaticScope,
    ICodeModelExecutionScope? ParentScope = null) : IInstantiatedObject
{
    public bool IsLiteralExpression => false;
    IBaseTypeDeclaration IInstantiatedObject.Type => Type;

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

    public IList<ICodeModelExecutionScope> GetScopes(ICodeModelExecutionContext context)
        => new[] { StaticScope, Scope };

    public void EnterScopes(ICodeModelExecutionContext context)
    {
        context.EnterScope(StaticScope);
        context.EnterScope(Scope);
    }

    public void ExitScopes(ICodeModelExecutionContext context)
    {
        context.ExitScope(Scope);
        context.ExitScope(StaticScope);
    }

    public IExpression Evaluate(ICodeModelExecutionContext context) => this;
    public object? EvaluatePlain(ICodeModelExecutionContext context) => this;

    public IExpression GetValue(string identifier) => Type.TryGetMember(identifier) switch
    {
        Field _ => Scope.GetValue(identifier),
        //Property property => Literal(property.AccessValue(_object)),
        Method _ => throw new CodeModelExecutionException($"Cannot get value of method '{identifier}'"),
        _ => throw new CodeModelExecutionException($"Cannot get non-found identifier '{identifier}'")
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

    public Argument ToArgument()
    {
        throw new NotImplementedException();
    }
    public ArgumentList ToArgumentList() => ToArgument().ToArgumentList();

    public ArgumentSyntax ToArgumentSyntax() => ToArgument().Syntax();

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