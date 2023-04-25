using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeAnalyzation.Models.Execution.ControlFlow;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;

public record ConstructorInvocationExpression(Constructor Constructor, List<IExpression> Arguments)
    : AnyArgExpression<InvocationExpressionSyntax>(Arguments.ToList(), Constructor.Type, OperationType.Invocation)
{
    public override InvocationExpressionSyntax Syntax() => InvocationExpressionCustom(Constructor.Name, Arguments.Select(x => x.Syntax()));

    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        if (Constructor.Body is null && Constructor.ExpressionBody is null)
        {
            throw new NotImplementedException();
        }
        var instance = Constructor.ClassType.CreateInstance();
        try
        {
            instance.EnterScopes(context);
            for (var i = 0; i < Constructor.Parameters.Properties.Count; i++)
            {
                var argument = Arguments.Count > i ? Arguments[i] : VoidValue;
                var parameter = Constructor.Parameters.Properties[i];
                context.DefineVariable(parameter.Name);
                context.SetValue(parameter.Name, argument == VoidValue ? parameter.Value : argument);
            }
            if (Constructor.Body is not null)
            {
                Constructor.Body.Evaluate(context);
            }
            else Constructor.ExpressionBody?.Evaluate(context);
        }
        finally
        {
            instance.ExitScopes(context);
        }
        return instance;
    }

    public override object? EvaluatePlain(IProgramModelExecutionContext context)
        => Evaluate(context);
}

public record ConstructorInvocationFromReflection(ConstructorInfo Constructor, IExpression Caller, List<IExpression> Arguments)
    : AnyArgExpression<InvocationExpressionSyntax>(new IExpression[] { Caller }.Concat(Arguments).ToList(), new TypeFromReflection(Constructor.DeclaringType), OperationType.Invocation)
{
    public override InvocationExpressionSyntax Syntax() => InvocationExpressionCustom(Caller.Syntax(), Arguments.Select(x => x.Syntax()));

    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        var arguments = Arguments.Select(x => x.EvaluatePlain(context)).ToArray();

        try
        {
            var argumentDiff = Constructor.GetParameters().Length - arguments.Length;
            if (argumentDiff > 0)
            {
                arguments = arguments.Concat(Enumerable.Range(0, argumentDiff)
                    .Select(x => System.Type.Missing)).ToArray();
            }
            var invocationResult = Constructor.Invoke(arguments);
            return Literal(invocationResult);
        }
        catch (TargetInvocationException e) when
            (e.InnerException is ObjectDisposedException innerException)
        {
            throw new ThrowException(innerException);
        }
    }
}