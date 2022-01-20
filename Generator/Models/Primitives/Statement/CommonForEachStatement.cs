using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public abstract record CommonForEachStatement(Block Block) : AbstractStatement<CommonForEachStatementSyntax>;
