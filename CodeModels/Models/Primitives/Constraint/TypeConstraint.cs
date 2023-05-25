using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record TypeConstraint(IType Type) : TypeParameterConstraint<TypeConstraintSyntax>
{
    public static TypeConstraint Create(IType type) => new(type);

    public override TypeConstraintSyntax Syntax() => TypeConstraint(Type.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }
}