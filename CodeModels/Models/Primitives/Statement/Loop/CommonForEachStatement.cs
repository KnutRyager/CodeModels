using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public abstract record CommonForEachStatement(Block Block) : AbstractStatement<CommonForEachStatementSyntax>;
