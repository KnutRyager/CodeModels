using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IMethodHolder : ICodeModel, ITypeModel, IMember
{
    new BaseTypeDeclarationSyntax Syntax();
    string Name { get; }
    bool IsStatic { get; }
}

public interface IMethodHolder<T> : IMethodHolder, ICodeModel<T> where T : BaseTypeDeclarationSyntax
{
    new T Syntax();
}
