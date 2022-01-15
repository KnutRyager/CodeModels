using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models;

public record ForEachStatement(IType? Type, string Identifier, IExpression Expression, IStatement Statement)
    : AbstractStatement<ForEachStatementSyntax>
{
    public ForEachStatement(string Identifier, IExpression Expression, IStatement Statement)
        : this(null, Identifier, Expression, Statement) { }

    public override ForEachStatementSyntax Syntax() => ForEachStatementCustom((Type ?? TypeShorthands.VarType).Syntax(),
        SyntaxFactory.Identifier(Identifier),
        Expression.Syntax(),
        Statement.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        if (Type is not null) yield return Type;
        yield return Expression;
        yield return Statement;
    }
}
