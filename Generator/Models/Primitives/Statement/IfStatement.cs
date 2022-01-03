using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models
{

    public record IfStatement(IEnumerable<AttributeListSyntax> AttributeLists, IExpression Condition, IStatement Statement, IStatement Else) : AbstractStatement<IfStatementSyntax>
    {
        public override IfStatementSyntax Syntax() => IfStatementCustom(AttributeLists,
            Condition.Syntax(), Statement.Syntax(), Else is null ? null : ElseClauseCustom(Else.Syntax()));
    }

    public record MultiIfStatement(List<IfStatement> IfStatements, IStatement? Else) : AbstractStatement<IfStatementSyntax>
    {
        public override IfStatementSyntax Syntax()
        {
            var ifs = IfStatements.First().Syntax();
            for (var i = 1; i < IfStatements.Count; i++)
            {
                ifs = ifs.WithElse(ElseClauseCustom(IfStatements[i].Syntax()));
            }
            if (Else is not null) ifs = ifs.WithElse(ElseClauseCustom(Else.Syntax()));
            return ifs;
        }
    }
}