using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public interface IMethod : IMember
    {
        MethodDeclarationSyntax ToMethodSyntax(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None);
    }
}
