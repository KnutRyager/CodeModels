using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record ConstructorConstraint() : TypeParameterConstraint<ConstructorConstraintSyntax>
{
    public static ConstructorConstraint Create() => new();

    public override ConstructorConstraintSyntax Syntax() => ConstructorConstraint();

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();
}