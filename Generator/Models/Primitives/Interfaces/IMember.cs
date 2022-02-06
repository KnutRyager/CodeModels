using System.Collections.Generic;
using CodeAnalyzation.Models.ProgramModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models;

public interface IMember : ICodeModel, ITypeModel
{
    string Name { get; }
    Modifier Modifier { get; }
    bool IsStatic { get; }
    new MemberDeclarationSyntax Syntax();
    MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None);
}

public interface IMember<T> : ICodeModel<T>, IMember where T : MemberDeclarationSyntax
{
    new T Syntax();
}

public abstract record MemberModel<T>(string Name, IType Type, List<AttributeList> Attributes, Modifier Modifier, IProgramContext? Context = null)
    : CodeModel<T>(Context), IMember<T> where T : MemberDeclarationSyntax
{
    public IType Get_Type() => Type;
    public virtual bool IsStatic => Modifier.HasFlag(Modifier.Static);
    public TypeSyntax TypeSyntax() => Get_Type().Syntax();
    MemberDeclarationSyntax IMember.Syntax() => Syntax();
    public abstract T SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None);
    MemberDeclarationSyntax IMember.SyntaxWithModifiers(Modifier modifier, Modifier removeModifier) => SyntaxWithModifiers(modifier, removeModifier);
    public override T Syntax() => SyntaxWithModifiers();
}
