using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Execution.Context;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeModels.Factory.CodeModelFactory;
using Microsoft.CodeAnalysis.CSharp;
using CodeModels.Generation;

namespace CodeModels.Models;

public record LocalFunctionStatement(string Identifier,
    ParameterList Parameters,
    List<IType> TypeParameters,
    List<TypeParameterConstraintClause> ConstraintClauses,
    IType ReturnType,
    Block? Body,
    IExpression? ExpressionBody,
    Modifier Modifier)
    : AbstractStatement<LocalFunctionStatementSyntax>(Identifier, Modifier)
{
    public static LocalFunctionStatement Create(string name,
            IToParameterListConvertible? parameters = null,
            IType? returnType = null,
            IEnumerable<IType>? typeParameters = null,
            IEnumerable<TypeParameterConstraintClause>? constraintClauses = null,
            Block? body = null,
            IExpression? expressionBody = null,
            Modifier? modifier = null)
            => new(name, parameters?.ToParameterList() ?? ParamList(), List(typeParameters), List(constraintClauses), returnType ?? TypeShorthands.VoidType, body, expressionBody, modifier ?? Modifier.Public);

    public override LocalFunctionStatementSyntax Syntax()
        => SyntaxFactoryCustom.LocalFunctionStatementCustom(
            attributeLists: new List<AttributeListSyntax>(),
            modifiers: Modifier.Syntax(),
            returnType: ReturnType.Syntax(),
            identifier: IdentifierSyntax(),
            typeParameterList: TypeParameters.Count is 0 ? null : TypeParameterList(SeparatedList(TypeParameters.Select(x => x.ToTypeParameter()))),
            parameterList: Parameters.Syntax(),
            constraintClauses: SyntaxFactory.List(ConstraintClauses.Select(x => x.Syntax())),
            body: Body?.Syntax(),
            expressionBody: ExpressionBody is null ? null! : ArrowExpressionClause(ExpressionBody.Syntax()));

    public override IEnumerable<ICodeModel> Children()
    {
        yield return ReturnType;
        foreach (var type in TypeParameters) yield return type;
        yield return Parameters;
        foreach (var constraintClause in ConstraintClauses) yield return constraintClause;
        if (Body is not null) yield return Body;
        if (ExpressionBody is not null) yield return ExpressionBody;
    }

    public override void Evaluate(ICodeModelExecutionContext context)
    {
        new LocalFunctionExpression(this).Evaluate(context);
    }

    public override IType Get_Type() => ReturnType;

    public virtual object? EvaluatePlain(ICodeModelExecutionContext context)
    {
        if (Parameters.Parameters.Count > 0)
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

    public Method ToMethod() => MethodFull(name: Name,
        parameters: Parameters,
        returnType: ReturnType,
        typeParameters: TypeParameters,
        constraintClauses: ConstraintClauses,
        body: Body,
        expressionBody: ExpressionBody,
        modifier: Modifier);
}
