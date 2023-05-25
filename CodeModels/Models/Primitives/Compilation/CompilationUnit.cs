using System.Collections.Generic;
using System.Linq;
using CodeModels.Models.Primitives.Attribute;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record CompilationUnit(
    List<IMember> Members,
    List<UsingDirective> Usings,
    List<AttributeList> Attributes,
    List<ExternAliasDirective>? Externs = null)
    : CodeModel<CompilationUnitSyntax>()
{
    public override CompilationUnitSyntax Syntax() => CompilationUnit(
        List((Externs ?? Enumerable.Empty<ExternAliasDirective>()).Select(x => x.Syntax())),
        List(Usings.Select(x => x.Syntax())),
        List(Attributes.Select(x => x.Syntax())),
        List(Members.Select(x => x.Syntax())));

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var member in Members) yield return member;
    }
}
