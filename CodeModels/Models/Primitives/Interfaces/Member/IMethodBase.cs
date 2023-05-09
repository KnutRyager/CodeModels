using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IMethodBase : IMember, IInvokable
{
    new BaseMethodDeclarationSyntax Syntax();
    new BaseMethodDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None);
}

public interface IMethodBase<T> : ICodeModel<T>, IMethodBase where T : BaseMethodDeclarationSyntax
{
    new T Syntax();
    new CodeModel<T> Render(Namespace @namespace);
}

public abstract record MethodBase<T, U>(IType Type, string Name, List<AttributeList> Attributes, Modifier Modifier)
    : MemberModel<T>(Type, Attributes, Modifier, Name), IMethodBase<T>, IInvokable<U>
    where T : BaseMethodDeclarationSyntax
    where U : IInvocation
{
    BaseMethodDeclarationSyntax IMethodBase.Syntax() => Syntax();
    BaseMethodDeclarationSyntax IMethodBase.SyntaxWithModifiers(Modifier modifier, Modifier removeModifier) => SyntaxWithModifiers(modifier, removeModifier);
    IInvocation IInvokable.Invoke(IExpression caller, IEnumerable<IExpression> arguments)
        => Invoke(caller, arguments);
    public abstract U Invoke(IExpression caller, IEnumerable<IExpression> arguments);
}
