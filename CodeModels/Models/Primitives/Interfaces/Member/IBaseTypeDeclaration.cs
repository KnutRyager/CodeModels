using System.Collections.Generic;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IBaseTypeDeclaration : ICodeModel, ITypeModel, IMember 
{
    List<IMember> Members { get; }
    List<Method> Methods();
    new BaseTypeDeclarationSyntax Syntax();
    IInstantiatedObject CreateInstance();
}

public interface IBaseTypeDeclaration<T> : ICodeModel<T>, IBaseTypeDeclaration
    where T : BaseTypeDeclarationSyntax
{
    new T Syntax();
    new List<IMember> Members { get; }
}
