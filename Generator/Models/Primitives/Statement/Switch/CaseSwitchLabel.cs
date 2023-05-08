﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public record CaseSwitchLabel(IExpression Value)
    : SwitchLabel<CaseSwitchLabelSyntax>
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Value;
    }

    public override bool Match(IProgramModelExecutionContext context, IExpression condition)
        => Equals(Value.Evaluate(context).LiteralValue(), condition.LiteralValue());

    public override CaseSwitchLabelSyntax Syntax()
        => SyntaxFactory.CaseSwitchLabel(Value.Syntax());
}