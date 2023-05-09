using System.Collections.Generic;
using CodeModels.Execution;
using CodeModels.Models.ErDiagram;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record PropertyModelExpression(PropertyModel Property, IExpression? Instance = null, IList<IProgramModelExecutionScope>? Scopes = null, ISymbol? Symbol = null) 
    : Expression<ExpressionSyntax>(Property.Type, Symbol),
    IPropertyModelExpression
{
    public ITypeDeclaration? Owner => Property.Owner;

    IBaseTypeDeclaration? IMemberAccess.Owner => Owner;

    public override ExpressionSyntax Syntax() => Property?.AccessSyntax(Instance) ?? Syntax();

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Instance is not null) yield return Instance;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context) => Property.EvaluateAccess(Instance.Evaluate(context) ?? Property.Owner?.ToExpression(), context);
    public override IdentifierExpression ToIdentifierExpression() => Instance is IdentifierExpression idetifier ? idetifier : base.ToIdentifierExpression();

    public void Assign(IExpression value, IProgramModelExecutionContext context, IList<IProgramModelExecutionScope> scopes) => Property.Assign(value, context, scopes);
}
