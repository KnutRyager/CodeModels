using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Attribute;

public record AttributeList(AttributeTargetSpecifier? Target, List<Attribute> Attributes)
    : CodeModel<AttributeListSyntax>, IToAttributeListConvertible
{
    public static AttributeList Create(AttributeTargetSpecifier? target = null, IEnumerable<IToAttributeConvertible>? attributes = null)
        => new(target, attributes is null ? List<Attribute>() : List(attributes.Select(x => x.ToAttribute())));

    public override AttributeListSyntax Syntax() => AttributeList(Target?.Syntax(), SeparatedList(Attributes.Select(x => x.Syntax())));
    public override IEnumerable<ICodeModel> Children()
    {
        if (Target is not null) yield return Target;
        foreach (var attribute in Attributes) yield return attribute;
    }

    public AttributeList ToAttributeList() => this;
    public AttributeListList ToAttributeListList() => AttributesList(Attributes);
}