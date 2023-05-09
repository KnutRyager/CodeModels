using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record PropertyModelExpressionFromSymbol(IPropertySymbol PropertySymbol, IExpression? Instance = null, IList<ICodeModelExecutionScope>? Scopes = null) 
    : Expression<ExpressionSyntax>(new TypeFromSymbol(PropertySymbol.Type), PropertySymbol),
    IPropertyModelExpression
{
    public PropertyModel Property => ProgramContext.GetContext(PropertySymbol).Get<PropertyModel>(PropertySymbol);
    public IBaseTypeDeclaration? Owner => Property.Owner;

    public override ExpressionSyntax Syntax() => Property?.AccessSyntax(Instance) ?? Syntax();

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Instance is not null) yield return Instance;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context) => Property.EvaluateAccess(Instance.Evaluate(context) ?? Property.Owner?.ToExpression(), context);
    public override IdentifierExpression ToIdentifierExpression() => Instance is IdentifierExpression idetifier ? idetifier : base.ToIdentifierExpression();

    public void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes) => Property.Assign(value, context, scopes);
}
