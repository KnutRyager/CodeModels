using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record DeclarationPattern(IType Type, IVariableDesignation Designation)
    : Pattern<DeclarationPatternSyntax>
{
    public static DeclarationPattern Create(IType type, IVariableDesignation designation)
        => new(type, designation);

    public override IEnumerable<ICodeModel> Children()
{
    yield return Type;
    yield return Designation;
}

public override DeclarationPatternSyntax Syntax()
    => SyntaxFactory.DeclarationPattern(Type.Syntax(), Designation.Syntax());

}