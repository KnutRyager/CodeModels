using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models.Primitives.Member;

public record PropertyExpressionFromSymbol(IPropertySymbol PropertySymbol, IExpression? Instance = null, IList<ICodeModelExecutionScope>? Scopes = null)
    : Expression<ExpressionSyntax>(TypeFromSymbol.Create(PropertySymbol.Type), PropertySymbol),
    IPropertyExpression
{
    public IProperty Property => ProgramContext.GetContext(PropertySymbol).Get<IProperty>(PropertySymbol);
    public IBaseTypeDeclaration? Owner => Property.Owner;

    public override ExpressionSyntax Syntax() => Property?.AccessSyntax(Instance) ?? Syntax();

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Instance is not null) yield return Instance;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context) => Property.EvaluateAccess(Instance?.Evaluate(context) ?? Property.Owner?.ToExpression(), context);
    public override IdentifierExpression ToIdentifierExpression() => Instance is IdentifierExpression idetifier ? idetifier : base.ToIdentifierExpression();

    public void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes) => Property.Assign(value, context, scopes);
}
