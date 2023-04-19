using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public record FieldModelExpression(FieldModel Field, IExpression? Instance = null)
    : Expression<ExpressionSyntax>(Field.Type), IAssignable, IMemberAccess
{
    public override ExpressionSyntax Syntax() => Field?.AccessSyntax(Instance) ?? Syntax();

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Instance is not null) yield return Instance;
    }

    public override object? EvaluatePlain(IProgramModelExecutionContext context)
    {
        return Evaluate(context).LiteralValue;
    }
    public override IExpression Evaluate(IProgramModelExecutionContext context) => Field.EvaluateAccess(context, Instance ?? Field.Owner?.ToExpression());
    public override IdentifierExpression ToIdentifierExpression() => Instance is IdentifierExpression identifier ? identifier : base.ToIdentifierExpression();

    public void Assign(IExpression value, IProgramModelExecutionContext context) => Field.Assign(value, context);
    public AssignmentExpression Assign(IExpression value) => Field.Assign(Instance, value);

    public override object? LiteralValue => EvaluatePlain(new ProgramModelExecutionContext());

    public IMethodHolder? Owner => Field.Owner;
}
