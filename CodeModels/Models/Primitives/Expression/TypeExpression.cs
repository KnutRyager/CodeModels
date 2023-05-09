using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record TypeExpression(IType Type) : Expression<TypeSyntax>(Type)
{
    public override TypeSyntax Syntax() => Type.Syntax();
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }
}
