using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record LocalFunctionStatement(Modifier Modifier, IType ReturnType, string Identifier, TypeCollection TypeParameters, PropertyCollection Parameters, List<TypeParameterConstraintClause> ConstraintClauses, Block? Body, IExpression? ExpressionBody)
    : AbstractStatement<LocalFunctionStatementSyntax>
{
    public override LocalFunctionStatementSyntax Syntax()
        => LocalFunctionStatement(
           modifiers: Modifier.Syntax(),
           returnType: ReturnType.Syntax(),
            identifier: Identifier(Identifier),
            typeParameterList: TypeParameters.Syntax(),
            parameterList: Parameters.ToParameters(),
           constraintClauses: List(ConstraintClauses.Select(x => x.Syntax())),
           body: Body?.Syntax()!,
           expressionBody: ExpressionBody is null ? null! : ArrowExpressionClause(ExpressionBody.Syntax()));

    public override IEnumerable<ICodeModel> Children()
    {
        yield return ReturnType;
        foreach (var type in TypeParameters.Types) yield return type;
        foreach (var parameter in Parameters.Properties) yield return parameter;
        foreach (var type in TypeParameters.Types) yield return type;
        foreach (var constraintClause in ConstraintClauses) yield return constraintClause;
        if (Body is not null) yield return Body;
        if (ExpressionBody is not null) yield return ExpressionBody;
    }

    public override void Evaluate(IProgramModelExecutionContext context)
    {
        new LocalFunctionExpression(this).Evaluate(context);
    }

    public override IType Get_Type() => ReturnType;

    public virtual object? EvaluatePlain(IProgramModelExecutionContext context)
    {
        if (Parameters.Properties.Count > 0)
        {
            throw new NotImplementedException();
        }
        else if (Body is Block block)
        {
            block.Evaluate(context);
            return context.PreviousExpression.EvaluatePlain(context);
        }
        else if (ExpressionBody is IExpression expression)
        {
            return expression.EvaluatePlain(context);
        }

        throw new NotImplementedException();
    }

}