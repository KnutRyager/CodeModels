using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public record PropertyExpression(Property Property, IExpression? Instance = null) : Expression<ExpressionSyntax>(Property.Type), IAssignable
{
    public override ExpressionSyntax Syntax() => Property?.AccessSyntax(Instance) ?? ((IExpression)this).Syntax();

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Instance is not null) yield return Instance;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context) => Property.EvaluateAccess(context, Instance);
    public override IdentifierExpression GetIdentifier() => Instance is IdentifierExpression idetifier ? idetifier : base.GetIdentifier();

    public virtual void Assign(IExpression value, IProgramModelExecutionContext context) => Property.Assign(value, context);
}
