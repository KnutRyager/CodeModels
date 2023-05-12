using System.Collections.Generic;
using CodeModels.Execution.Context;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models;

public record LocalDeclarationStatement(VariableDeclaration Declaration, Modifier Modifiers = Modifier.None) : AbstractStatement<LocalDeclarationStatementSyntax>
{
    public override LocalDeclarationStatementSyntax Syntax() => LocalDeclarationStatementCustom(Modifiers.Syntax(),
        Declaration.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Declaration;
    }

    public override void Evaluate(ICodeModelExecutionContext context)
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

    public override void Evaluate(ICodeModelExecutionContext context) => Declarations.Evaluate(context);
}
