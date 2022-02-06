﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record ExpressionStatement(IExpression Expression) : AbstractStatement<ExpressionStatementSyntax>, IExpression, IMember
{
    public string Name => "None";
    public Modifier Modifier => Modifier.None;
    public bool IsStatic => false;
    public IType Get_Type() => Expression.Get_Type();

    public bool IsLiteralExpression => throw new System.NotImplementedException();

    public LiteralExpressionSyntax? LiteralSyntax => throw new System.NotImplementedException();

    public object? LiteralValue => throw new System.NotImplementedException();

    public ExpressionStatement(IStatement statement)
        : this(statement is ExpressionStatement expressionStatement ? expressionStatement.Expression : new ExpressionStatement(statement)) { }
    public override ExpressionStatementSyntax Syntax() => ExpressionStatement(Expression.Syntax());

    ExpressionSyntax IExpression.Syntax() => Expression.Syntax();
    MemberDeclarationSyntax IMember.Syntax() => GlobalStatement(Syntax());
    public MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None) => GlobalStatement(Syntax());
    public TypeSyntax TypeSyntax() => Get_Type().Syntax();
    public ArgumentSyntax ToArgument() => Argument(Expression.Syntax());
    public EnumMemberDeclarationSyntax ToEnumValue(int? value = null)
    {
        throw new System.NotImplementedException();
    }

    public ExpressionStatement AsStatement() => Expression.AsStatement();

    public override void Evaluate(IProgramModelExecutionContext context) => context.SetPreviousExpression(Expression.Evaluate(context));
    IExpression IExpression.Evaluate(IProgramModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }

    public object? EvaluatePlain(IProgramModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public IdentifierExpression GetIdentifier() => new(Get_Type().Name, Get_Type());
}
