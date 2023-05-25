using System;
using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.AbstractCodeModels.Member;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis;

namespace CodeModels.ProgramModels;

public record ModelRegistry(IList<NamedValueCollection> Models, IList<InstanceClass> Singletons)
{
    public bool Contains(ISymbol symbol)
    {
        throw new System.NotImplementedException();
    }

    public IExpression GetSingleton(IType type)
    {
        throw new NotImplementedException();
        //return Singletons.FirstOrDefault(x => x.Type() == type);
    }

    public void Register(ISymbol symbol, ICodeModel model)
    {
        throw new System.NotImplementedException();
    }
}