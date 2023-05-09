using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IToClassConvertible
{
    ClassDeclarationSyntax ToClass(string? name = null, Modifier modifiers = Modifier.Public, Modifier memberModifiers = Modifier.Public);
}
