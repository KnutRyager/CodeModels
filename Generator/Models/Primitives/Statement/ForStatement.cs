using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public record ForStatement(VariableDeclaration Declaration, IExpression? Initializers, IExpression Condition, IExpression Incrementors, IStatement Statement) : AbstractStatement<ForStatementSyntax>
    {
        public override ForStatementSyntax Syntax() => ForStatementCustom(Declaration.Syntax(),
            Initializers is null ? List<ExpressionSyntax>() : List(Initializers.Syntax()),
            Condition.Syntax(),
            List(Incrementors.Syntax()),
            Statement.Syntax());

        public override IEnumerable<ICodeModel> Children()
        {
            yield return Declaration;
            yield return Condition;
            yield return Incrementors;
            yield return Statement;
            if (Initializers is not null) yield return Initializers;
        }
    }

    public record SimpleForStatement(string Variable, IExpression Limit, IStatement Statement)
        : ForStatement(
            CodeModelFactory.Declaration(Type("int"), Variable, CodeModelFactory.Literal(0)),
            null,
            BinaryExpression(CodeModelFactory.Identifier("i"), OperationType.LessThan, Limit),
            UnaryExpression(CodeModelFactory.Identifier("i"), OperationType.UnaryAddAfter),
            Statement);
}