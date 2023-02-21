using CodeAnalyzation.Models.Execution.Controlflow;
using CodeAnalyzation.Models.ProgramModels;
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
                return context.PreviousExpression.EvaluatePlain(context) ?? context.ConsoleOutput;
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
        return null;
    }
}
