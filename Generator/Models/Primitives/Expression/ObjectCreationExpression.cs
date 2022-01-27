using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

// TODO: Determine arguments vs initializer
public record ObjectCreationExpression(IType Type, PropertyCollection? Arguments, InitializerExpression? Initializer) : Expression<ObjectCreationExpressionSyntax>(Type)
{
    public override ObjectCreationExpressionSyntax Syntax() => ObjectCreationExpression(Type.Syntax() , Arguments?.ToArguments(), Initializer?.Syntax());
    //public  ImplicitObjectCreationExpressionSyntax ImplicitSyntax() => ImplicitObjectCreationExpression(Arguments?.ToArguments(), Initializer?.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override object? Evaluate(IProgramModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }
}

public record ImplicitObjectCreationExpression(IType Type, PropertyCollection Arguments, InitializerExpression? Initializer) : Expression<ImplicitObjectCreationExpressionSyntax>(Type)
{
    public override ImplicitObjectCreationExpressionSyntax Syntax() => ImplicitObjectCreationExpression(Arguments.ToArguments(), Initializer?.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override object? Evaluate(IProgramModelExecutionContext context)
    {
        throw new System.NotImplementedException();
    }
}
