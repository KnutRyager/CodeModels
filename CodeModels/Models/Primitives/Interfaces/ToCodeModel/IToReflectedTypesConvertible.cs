using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IToReflectedTypesConvertible
{
    ArgumentListSyntax ToArguments();
}
