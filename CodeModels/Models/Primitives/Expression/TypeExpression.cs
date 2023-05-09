using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record TypeExpression(IType Type) : Expression<TypeSyntax>(Type)
{
    public override TypeSyntax Syntax() => Type.Syntax();
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }
}
