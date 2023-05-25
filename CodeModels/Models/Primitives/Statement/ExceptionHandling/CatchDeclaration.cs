using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models;

public record CatchDeclaration(IType Type, string? Identifier) : CodeModel<CatchDeclarationSyntax>
{
    public static CatchDeclaration Create(IType type, string? identifier = null)
        => new(type, identifier);

    public override CatchDeclarationSyntax Syntax() => CatchDeclarationCustom(Type.Syntax(), Identifier);

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public void Evaluate(ICodeModelExecutionContext context, IExpression expression)
    {
        //context.EnterScope();
        //Statement.Evaluate(context);
        //if (context.HandleThrow())
        //{
        //    context.ExitScope();
        //    context.EnterScope();
        //}
        //if (Finally is not null)
        //{
        //    Finally.Evaluate(context);
        //}
    }
}
