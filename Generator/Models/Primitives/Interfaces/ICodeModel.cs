using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Models.ProgramModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzation.Models
{
    public interface ICodeModel
    {
        CSharpSyntaxNode Syntax();
        string Code();
        IEnumerable<ICodeModel> Children();
    }

    public interface ICodeModel<T> : ICodeModel where T : CSharpSyntaxNode
    {
        new T Syntax();
    }

    public abstract record CodeModel<T>(IProgramContext? Context = null) : ICodeModel<T> where T : CSharpSyntaxNode
    {
        public abstract T Syntax();
        CSharpSyntaxNode ICodeModel.Syntax() => Syntax();
        public string Code() => Syntax().NormalizeWhitespace().ToString();
        public abstract IEnumerable<ICodeModel> Children();
    }
}