using System.Collections.Generic;
using CodeModels.Execution;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record PropertyExpression(Property Property, IExpression? Instance = null) 
    : Expression<ExpressionSyntax>(Property.Type), IAssignable, IMemberAccess
{
    public IBaseTypeDeclaration? Owner => Property.Owner;

    public override ExpressionSyntax Syntax() => Property?.AccessSyntax(Instance) ?? ((IExpression)this).Syntax();

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Instance is not null) yield return Instance;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context) => Property.EvaluateAccess(context, Instance);
    public override IdentifierExpression ToIdentifierExpression() => Instance is IdentifierExpression idetifier ? idetifier : base.ToIdentifierExpression();

    public virtual void Assign(IExpression value, IProgramModelExecutionContext context, IList<IProgramModelExecutionScope> scopes) => Property.Assign(value, context, scopes);
}
