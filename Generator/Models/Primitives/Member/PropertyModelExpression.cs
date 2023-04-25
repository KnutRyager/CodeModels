using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public record PropertyModelExpression(PropertyModel Property, IExpression? Instance = null) 
    : Expression<ExpressionSyntax>(Property.Type), IAssignable, IMemberAccess
{
    public ITypeDeclaration? Owner => Property.Owner;

    IBaseTypeDeclaration? IMemberAccess.Owner => Owner;

    public override ExpressionSyntax Syntax() => Property?.AccessSyntax(Instance) ?? Syntax();

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Instance is not null) yield return Instance;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context) => Property.EvaluateAccess(context, Instance ?? Property.Owner?.ToExpression());
    public override IdentifierExpression ToIdentifierExpression() => Instance is IdentifierExpression idetifier ? idetifier : base.ToIdentifierExpression();

    public virtual void Assign(IExpression value, IProgramModelExecutionContext context) => Property.Assign(value, context);
}
