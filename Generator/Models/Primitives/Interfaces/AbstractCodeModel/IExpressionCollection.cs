using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IExpressionCollection :
    ITypeCollection,
    IToArgumentListConvertible,
    IToArrayCreationConvertible,
    IToListCreationConvertible
{
    TypeSyntax TypeSyntax();
    IType Get_Type();
    EnumDeclarationSyntax ToEnum(string name, bool isFlags = false, bool hasNoneValue = false);

}
