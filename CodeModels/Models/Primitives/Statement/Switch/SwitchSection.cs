using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models;

public record SwitchSection(List<ISwitchLabel> Labels, List<IStatement> Statements)
    : CodeModel<SwitchSectionSyntax>
{
    public static SwitchSection Create(IEnumerable<ISwitchLabel>? labels = null, IEnumerable<IStatement>? statements = null)
        => new(List(labels), List(statements));
    public static SwitchSection Create(IExpression label, IEnumerable<IStatement> statements)
        => Create(List(SwitchLabel(label)), statements);
    public static SwitchSection Create(IEnumerable<IExpression> labels, IStatement statement)
        => Create(labels.Select(SwitchLabel).ToArray(), new List<IStatement>() { statement });
    public static SwitchSection Create(IExpression label, IStatement statement)
        => Create(List(SwitchLabel(label)), new List<IStatement>() { statement });

    public override SwitchSectionSyntax Syntax() =>
        SyntaxFactory.SwitchSection(SyntaxFactory.List(Labels.Select(x => x.Syntax())), SyntaxFactory.List(Statements.Select(x => x.Syntax()).ToArray()));

    public SwitchSection WithBreak() => Statements.LastOrDefault()?.EndsInBreak() ?? false ? this :
        this with { Statements = Statements.Concat(new IStatement[] { Break() }).ToList() };

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var statement in Statements) yield return statement;
        foreach (var label in Labels) yield return label;
    }

    public virtual bool IsMatch(ICodeModelExecutionContext context, IExpression condition)
        => Labels.Any(x => x.Match(context, condition));

    public void Evaluate(ICodeModelExecutionContext context)
    {
        foreach (var statement in Statements) statement.Evaluate(context);
    }
}