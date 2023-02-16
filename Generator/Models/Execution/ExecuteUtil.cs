using System;
using System.IO;
using System.Linq;
using CodeAnalyzation.Models.ProgramModels;
using Common.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Parsing.ParseUtil;

namespace CodeAnalyzation.Models.Execution;

public static class ExecuteUtil
{
    public static object? Eval(this string str, string? key = null, SourceCodeKind kind = SourceCodeKind.Regular)
    {
        var model = str.ParseAndKeepSemanticModel(key, kind);
        var topLevelRewriter = new TopLevelStatementRewriter();
        var rewrittenCompilationUnit = (CompilationUnitSyntax)model.Compilation.GetVisit(topLevelRewriter);
        if (rewrittenCompilationUnit != model.Compilation)
        {
            model = rewrittenCompilationUnit.ToString().ParseAndKeepSemanticModel(key, kind);
        }
        ProgramContext.NewContext(model.Model);
        var compilationModel = CodeModelParsing.Parse(model.Compilation, model.Model);
        if (compilationModel.Members.Count >= 0)
        {
            var context = new ProgramModelExecutionContext();
            context.EnterScope();
            //IExpression? result = null;
            foreach (var member in compilationModel.Members)
            {
                if (member is ExpressionStatement expressionStatement)
                {
                    if (expressionStatement.Expression is InvocationFromReflection invocation && invocation.Method.Name is "Write" or "WriteLine")
                    {
                        var sw = new StringWriter();
                        Console.SetOut(sw);
                        Console.SetError(sw);
                        var method = invocation.Method;
                        var arguments = invocation.Arguments.Select(x => x.EvaluatePlain(context)).ToArray();
                        method.Invoke(null, arguments);
                        var result = sw.ToString();
                        context.SetPreviousExpression(new LiteralExpression(result));
                    }
                    else
                    {
                        context.SetPreviousExpression(expressionStatement.Expression.Evaluate(context));
                        //result = expressionStatement.Expression.Evaluate(context);
                    }
                }
                else if (member is ReturnStatement returnStatement)
                {
                    context.SetPreviousExpression(returnStatement.Expression.Evaluate(context));
                }
                else if (member is IStatement statement)
                {
                    statement.Evaluate(context);
                }
            }
            return context.PreviousExpression.EvaluatePlain(context);
        }
        return null;
    }
}
