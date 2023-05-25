using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record ClassOrStructConstraint(SyntaxKind Kind, SyntaxToken ClassOrStructKeyword)
    : TypeParameterConstraint<ClassOrStructConstraintSyntax>
{
    public static ClassOrStructConstraint Create(SyntaxKind kind, SyntaxToken classOrStructKeyword)
        => new(kind, classOrStructKeyword);

    public override ClassOrStructConstraintSyntax Syntax() => ClassOrStructConstraint(Kind, ClassOrStructKeyword);

    public override IEnumerable<ICodeModel> Children()
    {
        return Array.Empty<ICodeModel>();
        //foreach (var constraint in Constraints) yield return Constraints;
    }
}