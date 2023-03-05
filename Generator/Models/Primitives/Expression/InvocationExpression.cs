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

public record InvocationExpression(Method Method, IExpression Caller, List<IExpression> Arguments)
    : AnyArgExpression<InvocationExpressionSyntax>(new IExpression[] { Caller }.Concat(Arguments).ToList(), Method.ReturnType, OperationType.Invocation)
{
    public override InvocationExpressionSyntax Syntax() => InvocationExpressionCustom(Caller.Syntax(), Arguments.Select(x => x.Syntax()));
}


public record InvocationFromReflection(MethodInfo Method, IExpression Caller, List<IExpression> Arguments)
    : AnyArgExpression<InvocationExpressionSyntax>(new IExpression[] { Caller }.Concat(Arguments).ToList(), new TypeFromReflection(Method.ReturnType), OperationType.Invocation)
{
    public override InvocationExpressionSyntax Syntax() => InvocationExpressionCustom(Caller.Syntax(), Arguments.Select(x => x.Syntax()));

    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        var arguments = Arguments.Select(x => x.EvaluatePlain(context)).ToArray();
        if (Method is { Name: "Write" or "WriteLine" })
        {
            var consoleWriter = new StringWriter();
            Console.SetOut(consoleWriter);
            Console.SetError(consoleWriter);
            Method.Invoke(null, arguments);
            var result = consoleWriter.ToString();
            context.ConsoleWrite(result);
        }

        var instance = Caller.EvaluatePlain(context);
        if (instance is Delegate && instance.GetType() is Type t && (t.FullName.Contains("System.Func")
            || (t.IsGenericType && t.FullName.Contains("System.Action"))))
        {
            var declaringType = Method.DeclaringType;
            var genericFuncType = Method.DeclaringType.GetGenericTypeDefinition();
            var genericTypeArguments = declaringType.GenericTypeArguments;
            var objectFuncType = genericFuncType
                .MakeGenericType(genericTypeArguments.Select(x => typeof(object)).ToArray());
            var objectMethod = objectFuncType.GetMethod("Invoke");
            var instanceGeneric = Caller.EvaluatePlain(context);
            var invocationResultGeneric = objectMethod.Invoke(instanceGeneric, arguments);
            return Literal(invocationResultGeneric);
        }
        else
        {
            try
            {
                var argumentDiff = Method.GetParameters().Length - arguments.Length;
                if (argumentDiff > 0)
                {
                    arguments = arguments.Concat(Enumerable.Range(0, argumentDiff)
                        .Select(x => System.Type.Missing)).ToArray();
                }
                var invocationResult = Method.Invoke(instance, arguments);
                return Literal(invocationResult);
            }
            catch (TargetInvocationException e) when
                (e.InnerException is ObjectDisposedException innerException)
            {
                throw new ThrowException(innerException);
            }
        }
    }
}