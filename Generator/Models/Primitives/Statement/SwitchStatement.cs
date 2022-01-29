using System;
using System.Collections.Generic;
using System.Linq;
using Common.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;

public record SwitchStatement(IExpression Expression, List<SwitchSection> Sections) : AbstractStatement<SwitchStatementSyntax>
{
    public SwitchStatement(IExpression expression, List<SwitchSection> Sections, IStatement @default)
           : this(expression, CollectionUtil.Add(Sections.Select(x => x.WithBreak()), new DefaultSwitchSection(@default))) { }

    public override SwitchStatementSyntax Syntax()
        => SwitchStatementCustom(Expression.Syntax(), Sections.Select(x => x.Syntax()));

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
        foreach (var section in Sections) yield return section;
    }

    public override void Evaluate(IProgramModelExecutionContext context)
    {
        context.EnterScope();
        var condition = Expression.Evaluate(context);
        foreach (var section in Sections)
        {
            if (section.IsMatch(context, condition))
            {
                section.Evaluate(context);
                if (context.HandleReturn() || context.HandleThrow()) return;
                if (context.HandleBreak()) break;
            }
        }
        context.ExitScope();
    }
}

public record SwitchSection(List<IExpression> Labels, IStatement Statement) : CodeModel<SwitchSectionSyntax>
{
    public SwitchSection(IExpression label, IStatement statement) : this(List(label), statement) { }
    public override SwitchSectionSyntax Syntax() =>
        SwitchSectionCustom(Labels.Select(x => SwitchLabelCustom(x.Syntax())), Block(Statement).Syntax());
    public SwitchSection WithBreak() => Statement.EndsInBreak() ? this : this with { Statement = Block(Statement).Add(Break()) };
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Statement;
        foreach (var label in Labels) yield return label;
    }

    public virtual bool IsMatch(IProgramModelExecutionContext context, IExpression condition)
    {
        foreach (var label in Labels)
        {
            var labelValue = label.Evaluate(context).LiteralValue;
            if (Equals(labelValue, condition.LiteralValue)) return true;
        }
        return false;
    }

    public void Evaluate(IProgramModelExecutionContext context)
    {
        Statement.Evaluate(context);
    }
}

public record DefaultSwitchSection(IStatement Statement) : SwitchSection(new List<IExpression>(), Statement)
{
    public override SwitchSectionSyntax Syntax() => SwitchSectionCustom(DefaultSwitchLabelCustom(), Block(Statement).Syntax());

    public override bool IsMatch(IProgramModelExecutionContext _, IExpression __) => true;
}
