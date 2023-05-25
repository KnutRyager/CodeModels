using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Attribute;

public record AttributeList(AttributeTargetSpecifier? Target, List<Attribute> Attributes)
    : CodeModel<AttributeListSyntax>
{
    public static AttributeList Create(AttributeTargetSpecifier? target = null, IEnumerable<Attribute>? attributes = null)
        => new(target, List(attributes));

    public override AttributeListSyntax Syntax() => AttributeList(Target?.Syntax(), SeparatedList(Attributes.Select(x => x.Syntax())));
    public override IEnumerable<ICodeModel> Children()
    {
        if (Target is not null) yield return Target;
        foreach (var attribute in Attributes) yield return attribute;
    }
}