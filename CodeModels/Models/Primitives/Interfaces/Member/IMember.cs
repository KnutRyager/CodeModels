using System.Collections.Generic;
using CodeModels.Factory;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public interface IMember : ICodeModel, IIdentifiable, ITypeModel, INamed
{
    Modifier Modifier { get; }
    bool IsStatic { get; }
    new MemberDeclarationSyntax Syntax();
    MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None);
    ICodeModel Render(Namespace @namespace);
}

public interface IMember<T> : ICodeModel<T>, IMember where T : MemberDeclarationSyntax
{
    new T Syntax();
    new CodeModel<T> Render(Namespace @namespace);
}

public abstract record MemberModel<T>(IType Type, List<AttributeList> Attributes, Modifier Modifier, string? Name = null)
    : NamedCodeModel<T>(Name ?? Type.Name), IMember<T> where T : MemberDeclarationSyntax
{
    public IBaseTypeDeclaration? Owner { get; set; }
    public IType Get_Type() => Type;
    public virtual bool IsStatic => Modifier.HasFlag(Modifier.Static);

    public TypeSyntax TypeSyntax() => Get_Type().Syntax();
    MemberDeclarationSyntax IMember.Syntax() => Syntax();
    public abstract T SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None);
    MemberDeclarationSyntax IMember.SyntaxWithModifiers(Modifier modifier, Modifier removeModifier) => SyntaxWithModifiers(modifier, removeModifier);
    public override T Syntax() => SyntaxWithModifiers();
    public abstract CodeModel<T> Render(Namespace @namespace);
    ICodeModel IMember.Render(Namespace @namespace)
        => Render(@namespace);

    public virtual IType ToType() => Type;
    public virtual IExpression ToExpression() => CodeModelFactory.Identifier(Name);
    public virtual ParameterSyntax ToParameter() => SyntaxFactory.Parameter(ToIdentifier());
    public virtual TupleElementSyntax ToTupleElement() => SyntaxFactory.TupleElement(Type.Syntax(), ToIdentifier());
}
