using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Attribute;

public record AttributeListList(List<AttributeList> AttributeLists) : IToAttributeListListConvertible
{
    public static AttributeListList Create<T>(IEnumerable<T>? attributeLists = null) where T : IToAttributeListConvertible
        => new(attributeLists is null ? List<AttributeList>() : List(attributeLists.Select(x => x.ToAttributeList())));

    public SyntaxList<AttributeListSyntax> Syntax() => SyntaxFactory.List(AttributeLists.Select(x => x.Syntax()));

    public IEnumerable<ICodeModel> Children()
    {
        foreach (var attributeList in AttributeLists) yield return attributeList;
    }

    public AttributeListList ToAttributeListList() => this;
}