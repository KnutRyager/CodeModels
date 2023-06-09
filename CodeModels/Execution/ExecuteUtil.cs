﻿using System.Threading.Tasks;
using CodeModels.Execution.Context;
using CodeModels.Execution.ControlFlow;
using CodeModels.Extensions;
using CodeModels.Factory;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Parsing.ParseUtil;

namespace CodeModels.Execution;

public static class ExecuteUtil
{
    public static object? Eval(this string str, string? key = null, SourceCodeKind kind = SourceCodeKind.Regular, bool catchExceptions = false)
    {
        var model = str.ParseAndKeepSemanticModel(key, kind);
        var topLevelRewriter = new TopLevelStatementRewriter();
        var rewrittenCompilationUnit = (CompilationUnitSyntax)model.Compilation.GetVisit(topLevelRewriter);
        if (rewrittenCompilationUnit != model.Compilation)
        {
            model = rewrittenCompilationUnit.ToString().ParseAndKeepSemanticModel(key, kind);
        }
        var programContext = ProgramContext.NewContext(model.Compilation, model.Model);
        var parser = new CodeModelParser(model.Model, model.Compilation, programContext);
        var compilationModel = parser.Parse();
        if (compilationModel.Members.Count >= 0)
        {
            var context = new CodeModelExecutionContext(programContext);
            try
            {
                context.EnterScope();
                foreach (var member in compilationModel.Members)
                {
                    if (member is ExpressionStatement expressionStatement)
                    {
                        context.SetPreviousExpression(expressionStatement.Expression.Evaluate(context));
                    }
                    else if (member is IStatement statement)
                    {
                        statement.Evaluate(context);
                    }
                }
                var previousExpression = context.PreviousExpression?.EvaluatePlain(context);

                return previousExpression is not Task && previousExpression is not null ? previousExpression
                    : context.ConsoleOutput is not "" ? context.ConsoleOutput : null;
            }
            catch (ReturnException e)
            {
                return e.Value;
            }
            catch (ThrowException e)
            {
                if (catchExceptions) return e.InnerException;
                throw e;
            }
        }
        return null;
    }

    public static object? Eval(this IExpression expression, ICodeModelExecutionContext? context = null)
    {
        if (context is null)
        {
            context = new CodeModelExecutionContext(ProgramContext.NewContext());
            context.EnterScope();
            if (expression is IMemberAccess memberAccess)
            {
                if (memberAccess.Instance is IInstantiatedObject instance)
                    instance.EnterScopes(context);
                else if (memberAccess.Owner is IClassDeclaration c)
                    context.EnterScope(c.GetStaticScope());
            }
        }
        try
        {
            return expression.EvaluatePlain(context);
        }
        catch (ReturnException e)
        {
            return e.Value;
        }
        catch (ThrowException e)
        {
            return e.InnerException;
        }
    }
}
