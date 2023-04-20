using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IMethodHolder : ICodeModel, ITypeModel, IMember
{
    List<IMember> Members { get; }
    List<Method> Methods();
    new BaseTypeDeclarationSyntax Syntax();
}

public interface IMethodHolder<T> : IMethodHolder, ICodeModel<T> where T : BaseTypeDeclarationSyntax
{
    new T Syntax();
}
