using System.Collections.Generic;
using CodeModels.Models.ProgramModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record FieldModelExpressionFromSymbol(IFieldSymbol FieldSymbol, IExpression? Instance = null, IList<IProgramModelExecutionScope>? Scopes = null)
    : Expression<ExpressionSyntax>(new TypeFromSymbol(FieldSymbol.Type), FieldSymbol),
    IFieldModelExpression
{
    public FieldModel Field => ProgramContext.GetContext(FieldSymbol).Get<FieldModel>(FieldSymbol);
    public IBaseTypeDeclaration? Owner => Field.Owner;

    public override ExpressionSyntax Syntax() => Field?.AccessSyntax(Instance) ?? Syntax();

    public FieldModel GetField(IProgramModelExecutionContext context) => Field ?? context.ProgramContext.Get<FieldModel>(Symbol);
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Instance is not null) yield return Instance;
    }

    public override object? EvaluatePlain(IProgramModelExecutionContext context)
    {
        return Evaluate(context).LiteralValue();
    }
    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        try
        {
            context.EnterScopes(Scopes);
            return Field.EvaluateAccess(Instance ?? GetField(context).Owner?.ToExpression(), context);
        }
        finally
        {
            context.ExitScopes(Scopes);
        }
    }

    public override IdentifierExpression ToIdentifierExpression() => Instance is IdentifierExpression identifier ? identifier : base.ToIdentifierExpression();

    public void Assign(IExpression value, IProgramModelExecutionContext context, IList<IProgramModelExecutionScope> scopes) => Field.Assign(value, context, scopes);
    public AssignmentExpression Assign(IExpression value)
    {
        var context = new ProgramModelExecutionContext();
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

    public override object? LiteralValue() => EvaluatePlain(new ProgramModelExecutionContext());
}
