using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IToParameterListSyntaxConvertible
{
    ParameterListSyntax ToParameterListSyntax();
}
