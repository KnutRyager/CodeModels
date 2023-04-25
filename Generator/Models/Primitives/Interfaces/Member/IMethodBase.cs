using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IMethodBase : IMember
{
    new BaseMethodDeclarationSyntax Syntax();
    new BaseMethodDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None);
}

public interface IMethodBase<T> : ICodeModel<T>, IMethodBase where T : BaseMethodDeclarationSyntax
{
    new T Syntax();
    new CodeModel<T> Render(Namespace @namespace);
}

public abstract record MethodBase<T>(IType Type, string Name, List<AttributeList> Attributes, Modifier Modifier)
    : MemberModel<T>(Type, Attributes, Modifier, Name), IMethodBase<T> where T : BaseMethodDeclarationSyntax
{
    BaseMethodDeclarationSyntax IMethodBase.Syntax() => Syntax();
    BaseMethodDeclarationSyntax IMethodBase.SyntaxWithModifiers(Modifier modifier, Modifier removeModifier) => SyntaxWithModifiers(modifier, removeModifier);
}
