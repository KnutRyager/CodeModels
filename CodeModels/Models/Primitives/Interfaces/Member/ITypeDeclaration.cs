using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface ITypeDeclaration : IBaseTypeDeclaration
{
    new TypeDeclarationSyntax Syntax();
}

public interface ITypeDeclaration<T> : IBaseTypeDeclaration<T>, ITypeDeclaration where T : TypeDeclarationSyntax
{
    new T Syntax();
}
