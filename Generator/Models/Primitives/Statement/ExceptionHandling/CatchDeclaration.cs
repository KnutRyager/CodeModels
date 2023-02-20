using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Models.Execution.Controlflow;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;

public record CatchDeclaration(IType Type, string? Identifier) : CodeModel<CatchDeclarationSyntax>
{
    public override CatchDeclarationSyntax Syntax() => CatchDeclarationCustom(Type.Syntax(), Identifier);

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public void Evaluate(IProgramModelExecutionContext context, IExpression expression)
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
