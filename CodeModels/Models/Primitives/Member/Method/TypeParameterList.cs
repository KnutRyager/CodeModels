using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member;

public record TypeParameterList(List<IType> TypeParameters)
    : CodeModel<TypeParameterListSyntax>(), IToTypeParameterListConvertible
{
    public static TypeParameterList Create(IEnumerable<IType>? TypeParameters = default)
        => new(List(TypeParameters));

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var TypeParameter in TypeParameters) yield return TypeParameter;
    }

    public override TypeParameterListSyntax Syntax()
        => SyntaxFactory.TypeParameterList(SeparatedList(TypeParameters.Select(x => x.ToTypeParameter())));

    public TypeParameterList ToTypeParameterList() => this;
}
