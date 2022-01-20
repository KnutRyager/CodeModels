using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models;

public record TryStatement(IStatement Statement, List<CatchClause> CatchClauses, FinallyClause? Finally = null) : AbstractStatement<TryStatementSyntax>
{
    public override TryStatementSyntax Syntax() => TryStatementCustom(
         Block(Statement).Syntax(), CatchClauses.Select(x => x.Syntax()), Finally?.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Statement;
        foreach (var catchClause in CatchClauses)
        {
            yield return catchClause;
        }
        if (Finally is not null) yield return Finally;
    }
}

public record CatchDeclaration(IType Type, string? Identifier) : CodeModel<CatchDeclarationSyntax>
{
    public override CatchDeclarationSyntax Syntax() => CatchDeclarationCustom(Type.Syntax(), Identifier);

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }
}

public record CatchClause(IType Type, string? Identifier, IStatement Statement) : CodeModel<CatchClauseSyntax>
{
    public override CatchClauseSyntax Syntax() => CatchClauseCustom(
        CatchDeclarationCustom(Type.Syntax(), Identifier), Block(Statement).Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        yield return Statement;
    }
}

public record FinallyClause(IStatement Statement) : CodeModel<FinallyClauseSyntax>
{
    public override FinallyClauseSyntax Syntax() => FinallyClauseCustom(Block(Statement).Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Statement;
    }
}
