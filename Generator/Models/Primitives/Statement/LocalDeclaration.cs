using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Models.CodeModelFactory;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzation.Models
{

    public record LocalDeclarationStatement(VariableDeclaration Declaration, Modifier Modifiers = Modifier.None) : AbstractStatement<LocalDeclarationStatementSyntax>
    {
        public override LocalDeclarationStatementSyntax Syntax() => LocalDeclarationStatementCustom(Modifiers.Syntax(),
            Declaration.Syntax());

        public override IEnumerable<ICodeModel> Children()
        {
            yield return Declaration;
        }
    }
}