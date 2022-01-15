using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface ITypeModel
{
    TypeSyntax TypeSyntax();
    IType Type { get; }
}
