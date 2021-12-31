using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public interface ICodeModel
    {
        CSharpSyntaxNode SyntaxNode();
    }
}