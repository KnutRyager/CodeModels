using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models
{
    public record ContinueStatement() : AbstractStatement<ContinueStatementSyntax>
    {
        public override ContinueStatementSyntax Syntax() => ContinueCustom();
    }
}