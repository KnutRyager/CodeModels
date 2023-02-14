using CodeAnalyzation.Models.ProgramModels;
using Microsoft.CodeAnalysis;
using static CodeAnalyzation.Parsing.ParseUtil;

namespace CodeAnalyzation.Models.Execution;

public static class ExecuteUtil
{
    public static object? Eval(this string str, string? key = null, SourceCodeKind kind = SourceCodeKind.Regular)
    {
        ProgramContext.NewContext();
        var model = str.ParseAndKeepSemanticModel(key, kind);
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
                    context.SetPreviousExpression(expressionStatement.Expression.Evaluate(context));
                    //result = expressionStatement.Expression.Evaluate(context);
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
