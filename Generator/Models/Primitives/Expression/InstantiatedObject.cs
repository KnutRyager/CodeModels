using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;

public record InstantiatedObject(ClassModel2 Type,
    IProgramModelExecutionScope Scope,
    IProgramModelExecutionScope StaticScope,
    IProgramModelExecutionScope? ParentScope = null) : IExpression
{
    public bool IsLiteralExpression => throw new NotImplementedException();

    public LiteralExpressionSyntax? LiteralSyntax => throw new NotImplementedException();

    public object? LiteralValue => throw new NotImplementedException();

    public SimpleNameSyntax NameSyntax => throw new NotImplementedException();

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

    public IType Get_Type()
    {
        throw new NotImplementedException();
    }

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