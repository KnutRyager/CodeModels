using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models
{
    public record ForEachStatement(IType? Type, string Identifier, IExpression Expression, IStatement Statement)
        : AbstractStatement<ForEachStatementSyntax>
    {
        public ForEachStatement(string Identifier, IExpression Expression, IStatement Statement)
            : this(null, Identifier, Expression, Statement) { }

        public override ForEachStatementSyntax Syntax() => ForEachStatementCustom((Type ?? TypeShorthands.VarType).TypeSyntax(),
            SyntaxFactory.Identifier(Identifier),
            Expression.Syntax(),
            Statement.Syntax());
    }
}