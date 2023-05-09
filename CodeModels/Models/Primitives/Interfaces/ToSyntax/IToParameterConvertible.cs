using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IToParameterConvertible
{
    ParameterSyntax ToParameter();
}
