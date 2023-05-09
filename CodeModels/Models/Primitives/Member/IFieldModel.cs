using CodeModels.Models.Interfaces;

namespace CodeModels.Models;

public interface IFieldModel : IFieldOrProperty, IScopeHolder
{
    IType Type { get; }
}

