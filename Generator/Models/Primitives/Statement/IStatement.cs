using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public interface IStatement
    {
        StatementSyntax To();
    }

    public interface IStatement<T> : IStatement where T : StatementSyntax
    {
        new T To();
    }

}