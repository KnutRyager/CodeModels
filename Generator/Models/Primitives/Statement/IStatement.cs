using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public interface IStatement : ICodeModel
    {
        new StatementSyntax Syntax();
    }

    public interface IStatement<T> : IStatement, ICodeModel<T> where T : StatementSyntax
    {
        new T Syntax();
    }

    public abstract record AbstractStatement<T>() : CodeModel<T>, IStatement<T> where T : StatementSyntax
    {
        StatementSyntax IStatement.Syntax() => Syntax();
    }
}