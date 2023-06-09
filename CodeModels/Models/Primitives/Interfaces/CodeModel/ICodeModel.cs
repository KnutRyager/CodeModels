﻿using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeModels.Models;

public interface ICodeModel
{
    CSharpSyntaxNode Syntax();
    string Code();
    ISet<IType> Dependencies(ISet<IType>? set = null);
    IEnumerable<ICodeModel> Children();
}

public interface ICodeModel<T> : ICodeModel where T : CSharpSyntaxNode
{
    new T Syntax();
}

public abstract record CodeModel<T>() : ICodeModel<T> where T : CSharpSyntaxNode
{
    public abstract T Syntax();
    CSharpSyntaxNode ICodeModel.Syntax() => Syntax();
    public string Code() => Syntax().NormalizeWhitespace().ToString().Replace("\r\n", Environment.NewLine);
    public ISet<IType> Dependencies(ISet<IType>? set = null)
    {
        var dependencies = set ?? new HashSet<IType>();
        foreach (var child in Children())
        {
            child.Dependencies(dependencies);
        }
        return dependencies;
    }

    ISet<IType> ICodeModel.Dependencies(ISet<IType>? set) => Dependencies(set);

    public abstract IEnumerable<ICodeModel> Children();

    public override string ToString() => Syntax().ToString();
}
