using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{

    public record Block(List<IStatement> Statements) : AbstractStatement<BlockSyntax>
    {
        public Block(IEnumerable<IStatement>? statements) : this(List(statements)) { }
        public override BlockSyntax Syntax() => Block(Statements.Select(x => x.Syntax()));
    }

}