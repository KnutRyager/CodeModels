using System.Collections.Generic;
using System.Linq;
using CodeModels.Models.Primitives.Attribute;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeModels.Factory.CodeModelFactory;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeModels.Models;

public record CompilationUnit(
    List<IMember> Members,
    List<UsingDirective> Usings,
    AttributeListList Attributes,
    List<ExternAliasDirective>? Externs = null)
    : CodeModel<CompilationUnitSyntax>()
{
    public static CompilationUnit Create(IEnumerable<IMember> members,
        IEnumerable<UsingDirective>? usings = null,
        IToAttributeListListConvertible? attributes = null,
        IEnumerable<ExternAliasDirective>? externs = null)
        => new(List(members), 
            List(usings), 
            attributes?.ToAttributeListList() ?? AttributesList(),
            externs is null ? null : List(externs));

    public override CompilationUnitSyntax Syntax() => CompilationUnit(
        SyntaxFactory.List((Externs ?? Enumerable.Empty<ExternAliasDirective>()).Select(x => x.Syntax())),
        SyntaxFactory.List(Usings.Select(x => x.Syntax())),
        Attributes.Syntax(),
        SyntaxFactory.List(Members.Select(x => x.Syntax())));

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var member in Members) yield return member;
    }
}
