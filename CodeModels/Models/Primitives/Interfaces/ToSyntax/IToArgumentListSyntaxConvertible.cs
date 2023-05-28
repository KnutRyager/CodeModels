using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IToArgumentListSyntaxConvertible
{
    ArgumentListSyntax ToArguments();
}
