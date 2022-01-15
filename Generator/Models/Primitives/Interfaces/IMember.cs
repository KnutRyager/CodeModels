using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IMember
{
    string Name { get; }
    Modifier Modifier { get; }
    MemberDeclarationSyntax ToMemberSyntax(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None);
}
