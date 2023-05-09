using CodeModels.Models.Interfaces;

namespace CodeModels.Models;

public interface IField : IFieldOrProperty, IScopeHolder
{
    IType Type { get; }
}

