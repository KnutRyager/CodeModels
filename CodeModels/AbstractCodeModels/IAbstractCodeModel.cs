using System;
using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Models;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeModels.AbstractCodeModels;

public interface IAbstractCodeModel : ICodeModel
{
    ICodeModel ToCodeModel();
}

public interface IAbstractCodeModel<T> : IAbstractCodeModel, ICodeModel
    where T : ICodeModel
{
    T ToCodeModel(IAbstractCodeModelSettings? settings = null);
}

public interface IAbstractCodeModel<T, TSyntax> : IAbstractCodeModel<T>, ICodeModel<TSyntax>
    where T : ICodeModel
    where TSyntax : CSharpSyntaxNode
{
    new T ToCodeModel(IAbstractCodeModelSettings? settings = null);
}

public abstract record AbstractAbstractCodeModel<T> : IAbstractCodeModel<T>
    where T : ICodeModel
{
    private IAbstractCodeModelSettings? settings;

    public virtual IEnumerable<ICodeModel> Children() => ToCodeModel().Children();
    public string Code() => ToCodeModel().Code();
    public ISet<IType> Dependencies(ISet<IType>? set = null) => ToCodeModel().Dependencies(set);
    ICodeModel IAbstractCodeModel.ToCodeModel() => ToCodeModel();
    public CSharpSyntaxNode Syntax() => ToCodeModel().Syntax();
    public T ToCodeModel(IAbstractCodeModelSettings? settings = null)
    {
        if (settings is not null) this.settings = settings;
        return OnToCodeModel(this.settings);
    }
    protected abstract T OnToCodeModel(IAbstractCodeModelSettings? settings = null);
}

public abstract record AbstractAbstractCodeModel<T, TSyntax>
    : AbstractAbstractCodeModel<T>, IAbstractCodeModel<T, TSyntax>
    where T : ICodeModel<TSyntax>
    where TSyntax : CSharpSyntaxNode
{
    public new TSyntax Syntax() => ToCodeModel().Syntax();
    TSyntax ICodeModel<TSyntax>.Syntax() => Syntax();
    CSharpSyntaxNode ICodeModel.Syntax() => Syntax();
}