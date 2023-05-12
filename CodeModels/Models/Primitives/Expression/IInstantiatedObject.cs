using System;
using System.Collections.Generic;
using CodeModels.Execution;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models;

public interface IInstantiatedObject : IScopeHolder, IExpression
{
    IBaseTypeDeclaration Type { get; }
    void EnterScopes(ICodeModelExecutionContext context);
    void ExitScopes(ICodeModelExecutionContext context);
}

public interface IInstantiatedObject<T> : IInstantiatedObject where T : IBaseTypeDeclaration
{
    new T Type { get; }
}