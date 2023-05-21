using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public abstract record BaseType<T>(IType Type)
    : CodeModel<T>(), IBaseType<T> where T : BaseTypeSyntax
{
    BaseTypeSyntax IBaseType.Syntax() => Syntax();
}
