using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IMethod : IMember, ICodeModel<MethodDeclarationSyntax>
{
    MethodDeclarationSyntax ToMethodSyntax(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None);
}
