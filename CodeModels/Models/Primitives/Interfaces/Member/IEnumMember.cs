using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IEnumMember : IMember
{
    EnumMemberDeclarationSyntax EnumSyntax();
    IExpression Value { get; }
}

public interface IEnumMember<T> : IMember<T>, IEnumMember where T : MemberDeclarationSyntax
{
    new T Syntax();
    new CodeModel<T> Render(Namespace @namespace);
}