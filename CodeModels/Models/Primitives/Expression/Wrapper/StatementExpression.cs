﻿using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Attribute;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public abstract record StatementExpression(IStatement Statement, ISymbol? Symbol = null)
    : IExpression
{
    public bool IsLiteralExpression => false;
    public LiteralExpressionSyntax? LiteralSyntax() => null;
    public object? LiteralValue() => null;

    public SimpleNameSyntax NameSyntax() => throw new NotImplementedException();

    public virtual ExpressionStatement AsStatement() => new(Statement);
    public IEnumerable<ICodeModel> Children() => Statement.Children();
    public string Code() => Statement.Code();
    public ISet<IType> Dependencies(ISet<IType>? set = null) => Statement.Dependencies(set);
    public virtual IExpression Evaluate(ICodeModelExecutionContext context) => throw new NotImplementedException();
    public virtual object? EvaluatePlain(ICodeModelExecutionContext context) => throw new NotImplementedException();
    public virtual IdentifierExpression Identifier() => throw new NotImplementedException();
    public virtual IType Get_Type() => Statement.Get_Type();
    public ExpressionSyntax Syntax() => throw new NotImplementedException();
    ExpressionOrPatternSyntax IExpressionOrPattern.Syntax() => throw new NotImplementedException();
    public Argument ToArgument() => throw new NotImplementedException();
    public ArgumentList ToArgumentList() => ToArgument().ToArgumentList();
    public ArgumentSyntax ToArgumentSyntax() => throw new NotImplementedException();
    public AttributeArgument ToAttributeArgument() => CodeModelFactory.AttributeArg(this);
    public AttributeArgumentList ToAttributeArgumentList() => ToAttributeArgument().ToAttributeArgumentList();
    public EnumMemberDeclarationSyntax ToEnumValue(int? value = null) => throw new NotImplementedException();
    CSharpSyntaxNode ICodeModel.Syntax() => Statement.Syntax();

    public IdentifierNameSyntax IdentifierNameSyntax()
    {
        throw new NotImplementedException();
    }

    public SyntaxToken IdentifierSyntax()
    {
        throw new NotImplementedException();
    }

    public IdentifierExpression ToIdentifierExpression()
    {
        throw new NotImplementedException();
    }
}
