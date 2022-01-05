using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models
{

    public record TryStatement(IStatement Statement, List<CatchClause> CatchClauses, FinallyClause? Finally = null) : AbstractStatement<TryStatementSyntax>
    {
        public override TryStatementSyntax Syntax() => TryStatementCustom(
             Block(Statement).Syntax(), CatchClauses.Select(x => x.Syntax()), Finally?.Syntax());
    }

    public record CatchClause(IType Type, string? Identifier, IStatement Statement) : CodeModel<CatchClauseSyntax>
    {
        public override CatchClauseSyntax Syntax() => CatchClauseCustom(
            CatchDeclarationCustom(Type.TypeSyntax(), Identifier), Block(Statement).Syntax());
    }

    public record FinallyClause(IStatement Statement) : CodeModel<FinallyClauseSyntax>
    {
        public override FinallyClauseSyntax Syntax() => FinallyClauseCustom(Block(Statement).Syntax());
    }
}