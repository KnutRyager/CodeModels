﻿using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Attribute;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record ExpressionStatement(IExpression Expression)
    : AbstractStatement<ExpressionStatementSyntax>, IExpression, IMember
{
    public override IType Get_Type() => Expression.Get_Type();

    public bool IsLiteralExpression => Expression.IsLiteralExpression;

    public LiteralExpressionSyntax? LiteralSyntax() => Expression.LiteralSyntax();

    public object? LiteralValue() => Expression.LiteralValue();

    public ExpressionStatement(IStatement statement)
        : this(statement is ExpressionStatement expressionStatement ? expressionStatement.Expression : new ExpressionStatement(statement)) { }
    public override ExpressionStatementSyntax Syntax() => ExpressionStatement(Expression.Syntax());

    ExpressionSyntax IExpression.Syntax() => Expression.Syntax();
    ExpressionOrPatternSyntax IExpressionOrPattern.Syntax() => Expression.Syntax();
    MemberDeclarationSyntax IMember.Syntax() => GlobalStatement(Syntax());
    public override MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None) => GlobalStatement(Syntax());
    public override TypeSyntax TypeSyntax() => Get_Type().Syntax();
    public override Argument ToArgument() => Expression.ToArgument();
    public ArgumentSyntax ToArgumentSyntax() => ToArgument().Syntax();
    public AttributeArgument ToAttributeArgument() => CodeModelFactory.AttributeArg(ToExpression());
    public AttributeArgumentList ToAttributeArgumentList() => ToAttributeArgument().ToAttributeArgumentList();
    public EnumMemberDeclarationSyntax ToEnumValue(int? value = null)
    {
        throw new System.NotImplementedException();
    }

    public ExpressionStatement AsStatement() => Expression.AsStatement();

    public override void Evaluate(ICodeModelExecutionContext context) => context.SetPreviousExpression(Expression.Evaluate(context));

    public object? EvaluatePlain(ICodeModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public IdentifierExpression Identifier() => new(Get_Type().Name, Get_Type());

    IExpression IExpressionOrPattern.Evaluate(ICodeModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }
}
