using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IToArgumentListConvertible
{
    ArgumentListSyntax ToArguments();
}
