﻿using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution.Context;
using CodeModels.Execution.ControlFlow;
using CodeModels.Models.Primitives.Expression.Abstract;
using Common.Util;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models;

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

    public override void Evaluate(ICodeModelExecutionContext context)
    {
        context.EnterScope();
        var condition = Expression.Evaluate(context);
        foreach (var section in Sections)
        {
            if (section.IsMatch(context, condition))
            {
                try
                {
                    section.Evaluate(context);
                }
                catch (BreakException)
                {
                    break;
                }
            }
        }
        context.ExitScope();
    }
}