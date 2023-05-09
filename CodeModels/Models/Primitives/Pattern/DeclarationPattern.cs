using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public record DeclarationPattern(IType Type, IVariableDesignation Designation)
    : Pattern<DeclarationPatternSyntax>
{
    public override IEnumerable<ICodeModel> Children()
{
    yield return Type;
    yield return Designation;
}

public override DeclarationPatternSyntax Syntax()
    => SyntaxFactory.DeclarationPattern(Type.Syntax(), Designation.Syntax());

}