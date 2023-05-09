using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record TypePattern(IType Type)
    : Pattern<TypePatternSyntax>
{
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override TypePatternSyntax Syntax()
        => SyntaxFactory.TypePattern(Type.Syntax());
}
