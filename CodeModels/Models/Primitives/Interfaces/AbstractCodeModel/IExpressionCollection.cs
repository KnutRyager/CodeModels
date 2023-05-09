using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

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
