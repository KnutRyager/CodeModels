﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using Microsoft.CodeAnalysis.CSharp;
using System;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;

namespace CodeModels.Models;

public record ReflectionCall(Method Method, IExpression Caller, List<IExpression> Arguments)
    : AnyArgExpression<InvocationExpressionSyntax>(new IExpression[] { Caller }.Concat(Arguments).ToList(), Method.ReturnType, OperationType.Invocation)
{
    public override InvocationExpressionSyntax Syntax() => InvocationExpressionCustom(Caller.Syntax(), Arguments.Select(x => x.Syntax()));
}

public record FieldReflection(FieldInfo Field, IExpression Caller)
    : AnyArgExpression<MemberAccessExpressionSyntax>(List(new[] { Caller }), Type(Field.FieldType), OperationType.Field)
{
    public override MemberAccessExpressionSyntax Syntax()
        => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Caller.Syntax(), IdentifierName(Field.Name));

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        try
        {
            context.EnterScope(Caller);
            var result = Field.GetValue(Caller.EvaluatePlain(context));
            context.ExitScope(Caller);
            return Literal(result);
        }
        catch (Exception e)
        {
            context.Throw(e);
        }
        return VoidValue;
    }
}

public record PropertyReflection(PropertyInfo Property, IExpression Caller)
    : AnyArgExpression<MemberAccessExpressionSyntax>(List(new[] { Caller }), Type(Property.PropertyType), OperationType.Property)
{
    public override MemberAccessExpressionSyntax Syntax()
        => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Caller.Syntax(), IdentifierName(Property.Name));

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        context.EnterScope(Caller);
        var result = Property.GetValue(Caller.EvaluatePlain(context));
        context.ExitScope(Caller);
        return Literal(result);
    }
}