using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models;

public record SwitchSection(List<ISwitchLabel> Labels, List<IStatement> Statements)
    : CodeModel<SwitchSectionSyntax>
{
    public SwitchSection(IExpression label, IStatement statement)
        : this(new List<ISwitchLabel>() { new CaseSwitchLabel(label) }, new List<IStatement>() { statement }) { }

    public SwitchSection(IEnumerable<IExpression> labels, IEnumerable<IStatement> statements)
        : this(labels.Select(x => new CaseSwitchLabel(x)).ToList<ISwitchLabel>(), statements.ToList()) { }

    public SwitchSection(IExpression label, IEnumerable<IStatement> statements)
        : this(new List<ISwitchLabel>() { new CaseSwitchLabel(label) }, statements.ToList()) { }

    public override SwitchSectionSyntax Syntax() =>
        SyntaxFactory.SwitchSection(SyntaxFactory.List(Labels.Select(x => x.Syntax())), SyntaxFactory.List(Statements.Select(x => x.Syntax()).ToArray()));

    public SwitchSection WithBreak() => Statements.LastOrDefault()?.EndsInBreak() ?? false ? this :
        this with { Statements = Statements.Concat(new IStatement[] { Break() }).ToList() };

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var statement in Statements) yield return statement;
        foreach (var label in Labels) yield return label;
    }

    public virtual bool IsMatch(IProgramModelExecutionContext context, IExpression condition)
        => Labels.Any(x => x.Match(context, condition));

    public void Evaluate(IProgramModelExecutionContext context)
    {
        foreach (var statement in Statements) statement.Evaluate(context);
    }
}