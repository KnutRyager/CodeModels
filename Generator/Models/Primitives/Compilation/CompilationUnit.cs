using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

// TODO: Externs
public record CompilationUnit(List<IMember> Members, List<UsingDirective> Usings, List<AttributeList> Attributes)
    : CodeModel<CompilationUnitSyntax>()
{

    public override CompilationUnitSyntax Syntax() => CompilationUnit(List<ExternAliasDirectiveSyntax>(),
        List(Usings.Select(x => x.Syntax())),
        List(Attributes.Select(x => x.Syntax())),
        List(Members.Select(x => x.Syntax())));

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var member in Members) yield return member;
    }
}
