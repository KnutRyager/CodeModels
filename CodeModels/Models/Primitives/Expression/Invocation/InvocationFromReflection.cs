using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeModels.Execution.Context;
using CodeModels.Execution.ControlFlow;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models.Primitives.Expression.Invocation;

public record InvocationFromReflection(MethodInfo Method, IExpression Caller, List<IExpression> Arguments)
    : AnyArgExpression<InvocationExpressionSyntax>(new IExpression[] { Caller }.Concat(Arguments).ToList(), TypeFromReflection.Create(Method), OperationType.Invocation), IInvocation
{
    public static InvocationFromReflection Create(MethodInfo method, IExpression caller, IEnumerable<IExpression>? arguments = null)
        => new(method, caller, List(arguments));

    public override InvocationExpressionSyntax Syntax() => InvocationExpressionCustom(
        SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Caller.Syntax(), SyntaxFactory.IdentifierName(Method.Name)), Arguments.Select(x => x.Syntax()));

    public override IExpression Evaluate(ICodeModelExecutionContext context)
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
            || t.IsGenericType && t.FullName.Contains("System.Action")))
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