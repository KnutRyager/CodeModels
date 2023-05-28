using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models.Primitives.Member;

public record ConstructorInitializer(ArgumentList Arguments, bool IsBase)
    : CodeModel<ConstructorInitializerSyntax>()
{
    public static ConstructorInitializer Create(IToArgumentListConvertible? arguments = null, bool isBase = false)
        => new(arguments?.ToArgumentList() ?? ArgList(), isBase);

    public override ConstructorInitializerSyntax Syntax()
        => SyntaxFactory.ConstructorInitializer(IsBase
            ? SyntaxKind.BaseConstructorInitializer : SyntaxKind.ThisConstructorInitializer,
            Arguments.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Arguments;
    }

    public IExpression Evaluate(ICodeModelExecutionContext context)
    {
        throw new NotImplementedException();
    }
}
