using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member;

public record AccessorList(List<Accessor> Accessors)
    : CodeModel<AccessorListSyntax>(), IToAccessorListConvertible
{
    public static AccessorList Create(IEnumerable<IToAccessorConvertible>? accessors = default)
        => new(accessors is null ? List<Accessor>() : List(accessors.Select(x => x.ToAccessor())));

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var accessor in Accessors) yield return accessor;
    }

    public override AccessorListSyntax Syntax()
        => SyntaxFactory.AccessorList(SyntaxFactory.List(Accessors.Select(x => x.Syntax())));

    public AccessorList ToAccessorList() => this;
}
