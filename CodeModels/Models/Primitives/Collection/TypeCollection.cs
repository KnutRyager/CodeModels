using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record TypeCollection(List<IType> Types) : CodeModel<TypeParameterListSyntax>,
    ITypeCollection
{
    public TypeCollection(IEnumerable<IType>? values = null) : this(List(values)) { }

    public ExpressionCollection ToExpressions() => new(Types.Select(x => CodeModelFactory.Literal(x.Name)));

    public override TypeParameterListSyntax Syntax() => TypeParameterList(SeparatedList(Types.Select(x => x.ToTypeParameter())));

    public override IEnumerable<ICodeModel> Children() => Types;

    public IType BaseType()
    {
        throw new System.NotImplementedException();
    }

    public List<IType> AsList(IType? typeSpecifier = null)
    {
        throw new System.NotImplementedException();
    }
}


