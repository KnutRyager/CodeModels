using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Expression.Access;

public record ElementAccessExpression(IType Type, IExpression Caller, List<IExpression> Arguments)
    : AnyArgExpression<ElementAccessExpressionSyntax>(new IExpression[] { Caller }.Concat(Arguments).ToList(), Type, OperationType.Invocation), IAssignable
{

    public override ElementAccessExpressionSyntax Syntax() => ElementAccessExpression(Caller.Syntax(),
        BracketedArgumentList(SeparatedList(Arguments.Select(x => x.ToArgument()))));

    public void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes)
    {
        try
        {
            context.EnterScopes(scopes);
            var valuePlain = value.EvaluatePlain(context);
            var callerPlain = Inputs.First().EvaluatePlain(context);
            var args = Arguments.Select(x => x.EvaluatePlain(context)).ToArray();
            var arguments = new object?[] { args[0], valuePlain };
            var set_itemMethod = Type.GetReflectedType()?.GetMethod("set_Item");
            if (set_itemMethod is not null)
            {
                set_itemMethod.Invoke(callerPlain, arguments);
            }
            else
            {
                throw new System.NotImplementedException();
            }
        }
        finally
        {
            context.ExitScopes(scopes);
        }
    }
}

public record ImplicitElementAccessExpression(IType Type, List<IExpression> Arguments)
    : AnyArgExpression<ImplicitElementAccessSyntax>(new IExpression[] { new ThisExpression(Type) }.Concat(Arguments).ToList(), Type, OperationType.Invocation), IAssignable
{
    public override ImplicitElementAccessSyntax Syntax() => ImplicitElementAccess(
        BracketedArgumentList(SeparatedList(Arguments.Select(x => x.ToArgument()))));

    public void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes)
    {
        var valuePlain = value.EvaluatePlain(context);
        var callerPlain = Inputs.First().EvaluatePlain(context);
        var args = Arguments.Select(x => x.EvaluatePlain(context)).ToArray();
        var arguments = new object?[] { args[0], valuePlain };
        var set_itemMethod = Type.GetReflectedType()?.GetMethod("set_Item");
        if (set_itemMethod is not null)
        {
            set_itemMethod.Invoke(callerPlain, arguments);
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }
}