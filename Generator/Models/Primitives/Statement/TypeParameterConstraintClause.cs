using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record TypeParameterConstraintClause(string Name, List<ITypeParameterConstraint> Constraints) : CodeModel<TypeParameterConstraintClauseSyntax>
{
    public TypeParameterConstraintClause(string name, IEnumerable<ITypeParameterConstraint> constraints) : this(name, constraints.ToList()) { }
    public override TypeParameterConstraintClauseSyntax Syntax() => TypeParameterConstraintClause(
        name: IdentifierName(Name),
        constraints: SeparatedList(Constraints.Select(x => x.Syntax())));

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var constraint in Constraints) yield return constraint;
    }
}

public interface ITypeParameterConstraint : ICodeModel<TypeParameterConstraintSyntax> { }

public abstract record TypeParameterConstraint<T>() : CodeModel<T>, ITypeParameterConstraint where T : TypeParameterConstraintSyntax
{
    TypeParameterConstraintSyntax ICodeModel<TypeParameterConstraintSyntax>.Syntax() => Syntax();
}

public record ClassOrStructConstraint(SyntaxKind Kind, SyntaxToken ClassOrStructKeyword) : TypeParameterConstraint<ClassOrStructConstraintSyntax>
{
    public override ClassOrStructConstraintSyntax Syntax() => ClassOrStructConstraint(Kind, ClassOrStructKeyword);

    public override IEnumerable<ICodeModel> Children()
    {
        return Array.Empty<ICodeModel>();
        //foreach (var constraint in Constraints) yield return Constraints;
    }
}

public record ConstructorConstraint() : TypeParameterConstraint<ConstructorConstraintSyntax>
{
    public override ConstructorConstraintSyntax Syntax() => ConstructorConstraint();

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();
}

public record DefaultConstraint() : TypeParameterConstraint<DefaultConstraintSyntax>
{
    public override DefaultConstraintSyntax Syntax() => DefaultConstraint();

    public override IEnumerable<ICodeModel> Children() => Array.Empty<ICodeModel>();
}

public record TypeConstraint(IType Type) : TypeParameterConstraint<TypeConstraintSyntax>
{
    public override TypeConstraintSyntax Syntax() => TypeConstraint(Type.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }
}