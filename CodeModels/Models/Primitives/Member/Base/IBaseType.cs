using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models
{
    public interface IBaseType : ICodeModel
    {
        IType Type { get; init; }

        new BaseTypeSyntax Syntax();
    }

    public interface IBaseType<T> : IBaseType
        where T : BaseTypeSyntax
    {
        new T Syntax();
    }
}