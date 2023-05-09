using System.Collections.Generic;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record WhenClause(IExpression Condition)
    : CodeModel<WhenClauseSyntax>
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Condition;
    }

    public override WhenClauseSyntax Syntax()
        => SyntaxFactory.WhenClause(Condition.Syntax());
}