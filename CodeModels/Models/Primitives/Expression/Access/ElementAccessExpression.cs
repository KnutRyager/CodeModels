using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Expression.Access;

public record ElementAccessExpression(IType Type, IExpression Caller, ArgumentList Arguments)
    : AnyArgExpression<ElementAccessExpressionSyntax>(new IExpression[] {
        This(Type) }.Concat(Arguments.Arguments.Select(x => x.Expression)).ToList(),
        Type, OperationType.Invocation), IAssignable
{
    public static ElementAccessExpression Create(IType type, IExpression caller, IEnumerable<IExpression>? arguments = null)
        => new(type, caller, arguments is null ? ArgList() : ArgList(arguments));

    public override ElementAccessExpressionSyntax Syntax() => ElementAccessExpression(Caller.Syntax(),
        Arguments.ToBracketedListSyntax());

    public void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes)
    {
        try
        {
            context.EnterScopes(scopes);
            var valuePlain = value.EvaluatePlain(context);
            var callerPlain = Inputs.First().EvaluatePlain(context);
            var args = Arguments.EvaluatePlain(context);
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