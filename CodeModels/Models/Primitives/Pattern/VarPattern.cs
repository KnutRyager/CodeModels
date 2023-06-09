﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record VarPattern(IVariableDesignation Designation)
    : Pattern<VarPatternSyntax>
{
    public static VarPattern Create(IVariableDesignation designation)
        => new(designation);

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Designation;
    }

    public override VarPatternSyntax Syntax()
        => SyntaxFactory.VarPattern(Designation.Syntax());
}
