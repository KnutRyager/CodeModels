using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IToArgumentListConvertible
{
    ArgumentListSyntax ToArguments();
}
