using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record AbstractPropertyExpression(AbstractProperty Property, IExpression? Instance = null) 
    : Expression<ExpressionSyntax>(Property.Type), IAssignable, IMemberAccess
{
    public IBaseTypeDeclaration? Owner => Property.Owner;

    public override ExpressionSyntax Syntax() => Property?.AccessSyntax(Instance) ?? ((IExpression)this).Syntax();

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Instance is not null) yield return Instance;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context) => Property.EvaluateAccess(context, Instance);
    public override IdentifierExpression ToIdentifierExpression() => Instance is IdentifierExpression idetifier ? idetifier : base.ToIdentifierExpression();
    public virtual void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes) => Property.Assign(value, context, scopes);
}
