using System.Collections.Generic;
using CodeModels.Models;
using Microsoft.CodeAnalysis;

namespace CodeModels.ProgramModels;

public record ModelRegistry(IList<PropertyCollection> Models, IList<InstanceClass> Singletons)
{
    public bool Contains(ISymbol symbol)
    {
        throw new System.NotImplementedException();
    }

    public IExpression GetSingleton(IType type)
    {
        return null;
        //return Singletons.FirstOrDefault(x => x.Type() == type);
    }

    public void Register(ISymbol symbol, ICodeModel model)
    {
        throw new System.NotImplementedException();
    }
}