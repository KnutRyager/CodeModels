using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IToClassConvertible
{
    ClassDeclarationSyntax ToClass(string? name = null, Modifier? modifiers = null, Modifier memberModifiers = Modifier.Public);
}
