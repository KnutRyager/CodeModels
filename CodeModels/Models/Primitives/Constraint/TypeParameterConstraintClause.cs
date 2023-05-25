using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record TypeParameterConstraintClause(string Name, List<ITypeParameterConstraint> Constraints)
    : CodeModel<TypeParameterConstraintClauseSyntax>
{
    public static TypeParameterConstraintClause Create(string name, IEnumerable<ITypeParameterConstraint>? constraints = null)
        => new(name, List(constraints));

    public override TypeParameterConstraintClauseSyntax Syntax() => TypeParameterConstraintClause(
        name: IdentifierName(Name),
        constraints: SeparatedList(Constraints.Select(x => x.Syntax())));

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var constraint in Constraints) yield return constraint;
    }
}