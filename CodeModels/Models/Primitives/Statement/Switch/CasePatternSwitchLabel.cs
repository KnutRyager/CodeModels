using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CodeModels.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record CasePatternSwitchLabel(IPattern Pattern, WhenClause? WhenClause)
    : SwitchLabel<CasePatternSwitchLabelSyntax>
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Pattern;
        if (WhenClause is not null) yield return WhenClause;
    }

    public override bool Match(IProgramModelExecutionContext context, IExpression condition)
        => throw new NotImplementedException();
    //=> Pattern.Equals(condition);

    public override CasePatternSwitchLabelSyntax Syntax()
        => SyntaxFactory.CasePatternSwitchLabel(Pattern.Syntax(), WhenClause?.Syntax(), SyntaxFactory.Token(SyntaxKind.ColonToken));
}