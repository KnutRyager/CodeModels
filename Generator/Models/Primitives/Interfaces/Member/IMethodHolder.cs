using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IMethodHolder : ICodeModel, ITypeModel, IMember
{
    List<IFieldOrProperty> Members { get; }
    List<IMethod> Methods { get; }
    new BaseTypeDeclarationSyntax Syntax();
}

public interface IMethodHolder<T> : IMethodHolder, ICodeModel<T> where T : BaseTypeDeclarationSyntax
{
    new T Syntax();
}
