using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;

namespace CodeAnalyzation.Models;

public record LocalDeclarationStatement(VariableDeclaration Declaration, Modifier Modifiers = Modifier.None) : AbstractStatement<LocalDeclarationStatementSyntax>
{
    public override LocalDeclarationStatementSyntax Syntax() => LocalDeclarationStatementCustom(Modifiers.Syntax(),
        Declaration.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Declaration;
    }
}
