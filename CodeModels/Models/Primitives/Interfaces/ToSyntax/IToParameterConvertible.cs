using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IToParameterConvertible
{
    ParameterSyntax ToParameter();
}
