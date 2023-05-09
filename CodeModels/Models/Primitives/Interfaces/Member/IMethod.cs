using CodeAnalyzation.Models.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IMethod : IMember, ICodeModel<MethodDeclarationSyntax>, IMethodInfo
{
    MethodDeclarationSyntax ToMethodSyntax(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None);
}
