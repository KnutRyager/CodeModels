using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IToReflectedTypesConvertible
{
    ArgumentListSyntax ToArguments();
}
