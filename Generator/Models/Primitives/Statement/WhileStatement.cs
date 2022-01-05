using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models
{
    public record WhileStatement(IExpression Condition, IStatement Statement) : AbstractStatement<WhileStatementSyntax>
    {
        public override WhileStatementSyntax Syntax() => WhileStatementCustom(Condition.Syntax(), Statement.Syntax());
    }

}