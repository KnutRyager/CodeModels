using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record DefaultConstraint() : TypeParameterConstraint<DefaultConstraintSyntax>
{
    public static DefaultConstraint Create() => new();

    public override DefaultConstraintSyntax Syntax() => DefaultConstraint();

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();
}