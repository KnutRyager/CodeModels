using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static CodeModels.Factory.CodeModelFactory;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models;

public record DefaultSwitchSection(IStatement Statement)
    : SwitchSection(new List<IExpression>(), new List<IStatement>() { Statement })
{
    public override SwitchSectionSyntax Syntax() => SwitchSectionCustom(DefaultSwitchLabelCustom(), Block(Statement).Syntax());

    public override bool IsMatch(ICodeModelExecutionContext _, IExpression __) => true;
}
