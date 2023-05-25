using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public abstract record TypeParameterConstraint<T>()
    : CodeModel<T>, ITypeParameterConstraint where T : TypeParameterConstraintSyntax
{
    TypeParameterConstraintSyntax ICodeModel<TypeParameterConstraintSyntax>.Syntax() => Syntax();
}