using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Expression.Access;

public record ImplicitElementAccessExpression(IType Type, ArgumentList Arguments)
    : AnyArgExpression<ImplicitElementAccessSyntax>(new IExpression[] {
        This(Type) }.Concat(Arguments.Arguments.Select(x => x.Expression)).ToList(),
        Type, OperationType.Invocation), IAssignable
{
    public static ImplicitElementAccessExpression Create(IType type, IEnumerable<IToArgumentConvertible>? arguments = null)
        => new(type, arguments is null ? ArgList() : ArgList(arguments));

    public override ImplicitElementAccessSyntax Syntax()
        => ImplicitElementAccess(Arguments.ToBracketedListSyntax());

    public void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes)
    {
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
}