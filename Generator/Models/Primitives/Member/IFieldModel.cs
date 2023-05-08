namespace CodeAnalyzation.Models;

public interface IFieldModel : IFieldOrProperty, IScopeHolder
{
    IType Type { get; }
}

