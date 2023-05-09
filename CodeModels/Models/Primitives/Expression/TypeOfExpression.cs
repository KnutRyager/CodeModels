using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record TypeOfExpression(IType Type) : Expression<TypeOfExpressionSyntax>(Type)
{
    public override TypeOfExpressionSyntax Syntax() => TypeOfExpression(Type.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context) => Type.GetReflectedType() is Type type ? new LiteralExpression(type) : throw new NotImplementedException();
}
