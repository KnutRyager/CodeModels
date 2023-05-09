using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IToParametersConvertible
{
    ParameterListSyntax ToParameters();
}
