using System.Collections.Generic;
using System.Linq;
using CodeModels.Factory;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Attribute;

public record AttributeListList(List<AttributeList> AttributeLists)
{
    public static AttributeListList Create(IEnumerable<AttributeList>? attributeLists = null)
        => new(CodeModelFactory.List(attributeLists));

    public SyntaxList<AttributeListSyntax> Syntax() => SyntaxFactory.List(AttributeLists.Select(x => x.Syntax()));

    public IEnumerable<ICodeModel> Children()
    {
        foreach (var attributeList in AttributeLists) yield return attributeList;
    }
}