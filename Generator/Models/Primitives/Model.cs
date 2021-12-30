using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public interface ICodeModel
    {
        TypeSyntax TypeSyntax();
    }
}