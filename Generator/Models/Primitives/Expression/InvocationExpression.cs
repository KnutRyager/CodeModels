using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        return Literal(Method.Invoke(Caller.EvaluatePlain(context), arguments));
    }
}