using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface ITypeModel
{
    TypeSyntax TypeSyntax();
    IType Get_Type();
}
