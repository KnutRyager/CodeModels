using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IConstructor : IMember, ICodeModel<ConstructorDeclarationSyntax>
{
    //ConstructorDeclarationSyntax ToMethodSyntax(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None);
}
