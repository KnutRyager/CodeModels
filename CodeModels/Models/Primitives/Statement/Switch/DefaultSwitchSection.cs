using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static CodeModels.Models.CodeModelFactory;

namespace CodeModels.Models;

public record DefaultSwitchSection(IStatement Statement)
    : SwitchSection(new List<IExpression>(), new List<IStatement>() { Statement })
{
    public override SwitchSectionSyntax Syntax() => SwitchSectionCustom(DefaultSwitchLabelCustom(), Block(Statement).Syntax());

    public override bool IsMatch(IProgramModelExecutionContext _, IExpression __) => true;
}
