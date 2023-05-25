using System;
using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Expression.Instantiation;

public record ImplicitObjectCreationExpression(IType Type, NamedValueCollection Arguments, InitializerExpression? Initializer) : Expression<ImplicitObjectCreationExpressionSyntax>(Type)
{
    public override ImplicitObjectCreationExpressionSyntax Syntax() => ImplicitObjectCreationExpression(Arguments.ToArguments(), Initializer?.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        throw new NotImplementedException();
    }
}
