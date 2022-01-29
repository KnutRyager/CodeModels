﻿using System.Collections.Generic;
using System.Linq;
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

    public override void Evaluate(IProgramModelExecutionContext context)
    {
        Declaration.Evaluate(context);
    }
}

public record LocalDeclarationStatements(VariableDeclarations Declarations, Modifier Modifiers = Modifier.None) : AbstractStatement<LocalDeclarationStatementSyntax>
{
    public override LocalDeclarationStatementSyntax Syntax() => LocalDeclarationStatementCustom(Modifiers.Syntax(), Declarations.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Declarations;
    }

    public override void Evaluate(IProgramModelExecutionContext context) => Declarations.Evaluate(context);
}
