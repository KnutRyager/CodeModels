using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{

    public record ForStatement(IStatement Statement, IExpression Condition, IExpression Initializer, IExpression Incrementors, VariableDeclaration Declaration) : AbstractStatement<ForStatementSyntax>
    {
        //public ForStatement(ForStatementSyntax Syntax) { }
        public override ForStatementSyntax Syntax() => ForStatementCustom(Declaration.Syntax(),
            List(Initializer.Syntax()),
            Condition.Syntax(),
            List(Incrementors.Syntax()),
            Statement.Syntax());
    }
}