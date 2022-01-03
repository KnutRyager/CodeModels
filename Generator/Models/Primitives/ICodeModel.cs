using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzation.Models
{
    public interface ICodeModel
    {
        CSharpSyntaxNode Syntax();
    }

    public interface ICodeModel<T> : ICodeModel where T : CSharpSyntaxNode
    {
        new T Syntax();
    }

    public abstract record CodeModel<T> : ICodeModel<T> where T : CSharpSyntaxNode
    {
        public abstract T Syntax();
        CSharpSyntaxNode ICodeModel.Syntax() => Syntax();
    }
}