using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models.Primitives.Member;

public record FieldExpressionFromSymbol(IFieldSymbol FieldSymbol, IExpression? Instance = null, IList<ICodeModelExecutionScope>? Scopes = null)
    : Expression<ExpressionSyntax>(TypeFromSymbol.Create(FieldSymbol.Type), FieldSymbol),
    IFieldExpression<Field>
{
    public Field Field => ProgramContext.GetContext(FieldSymbol)?.Get<Field>(FieldSymbol) ?? throw new NotImplementedException();
    public IBaseTypeDeclaration? Owner => Field.Owner;
    IField IFieldExpression.Field => Field;

    public override ExpressionSyntax Syntax() => Field?.AccessSyntax(Instance) ?? Syntax();

    public Field GetField(ICodeModelExecutionContext context) => Field ?? context.ProgramContext.Get<Field>(FieldSymbol);
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Instance is not null) yield return Instance;
    }

    public override object? EvaluatePlain(ICodeModelExecutionContext context)
    {
        return Evaluate(context).LiteralValue();
    }
    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        try
        {
            context.EnterScopes(Scopes);
            return Field.EvaluateAccess(Instance ?? GetField(context).Owner!.ToExpression(), context);
        }
        finally
        {
            context.ExitScopes(Scopes);
        }
    }

    public override IdentifierExpression ToIdentifierExpression() => Instance is IdentifierExpression identifier ? identifier : base.ToIdentifierExpression();

    public void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes) => Field.Assign(value, context, scopes);
    public AssignmentExpression Assign(IExpression value)
    {
        var context = new CodeModelExecutionContext();
        try
        {
            context.EnterScopes(Scopes);
            return Field.Assign(Instance, value);
        }
        finally
        {
            context.ExitScopes(Scopes);
        }
    }

    public override object? LiteralValue() => EvaluatePlain(new CodeModelExecutionContext());
}
