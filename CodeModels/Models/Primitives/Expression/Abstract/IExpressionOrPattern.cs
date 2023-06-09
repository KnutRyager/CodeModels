﻿using CodeModels.Execution.Context;
using CodeModels.Models.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models.Primitives.Expression.Abstract;

public interface IExpressionOrPattern : ICodeModel, IIdentifiable
{
    IType Get_Type();
    IExpression Evaluate(ICodeModelExecutionContext context);
    object? EvaluatePlain(ICodeModelExecutionContext context);
    new ExpressionOrPatternSyntax Syntax();
}

public interface IExpressionOrPattern<T> : IExpressionOrPattern, ICodeModel<T> where T : ExpressionOrPatternSyntax
{
    new T Syntax();
}
