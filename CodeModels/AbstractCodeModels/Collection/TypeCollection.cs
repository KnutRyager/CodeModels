using System.Collections.Generic;
using System.Linq;
using CodeModels.Factory;
using CodeModels.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.AbstractCodeModels.Collection;

public record TypeCollection(List<IType> Types) : CodeModel<TypeParameterListSyntax>,
    ITypeCollection
{
    public static TypeCollection Create(IEnumerable<IType>? values = null) => new(List(values));

    public ExpressionCollection ToExpressions() => AbstractCodeModelFactory.Expressions(Types.Select(x => CodeModelFactory.Literal(x.Name)));

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


